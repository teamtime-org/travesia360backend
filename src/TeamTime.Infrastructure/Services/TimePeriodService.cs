using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTime.Application.Services;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Services;

public class TimePeriodService : ITimePeriodService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TimePeriodService> _logger;

    public TimePeriodService(ApplicationDbContext context, ILogger<TimePeriodService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TimePeriod> GetCurrentPeriodAsync(PeriodType periodType)
    {
        var currentPeriod = await _context.TimePeriods
            .FirstOrDefaultAsync(tp => tp.Type == periodType && tp.IsCurrent && tp.IsActive);

        if (currentPeriod == null)
        {
            throw new InvalidOperationException($"No current {periodType} period found");
        }

        return currentPeriod;
    }

    public async Task<TimePeriod> GetOrCreateCurrentPeriodAsync(PeriodType periodType, decimal? referenceHours = null)
    {
        var today = DateTime.UtcNow.Date;

        // First, try to find an existing current period
        var currentPeriod = await _context.TimePeriods
            .FirstOrDefaultAsync(tp => tp.Type == periodType && tp.IsCurrent && tp.IsActive);

        if (currentPeriod != null && currentPeriod.ContainsDate(today))
        {
            return currentPeriod;
        }

        // If current period doesn't contain today, find or create the correct period
        var periodForToday = await FindPeriodForDateAsync(today, periodType);

        if (periodForToday != null)
        {
            // Mark as current and deactivate old current period
            await SetCurrentPeriodAsync(periodForToday.Id);
            return periodForToday;
        }

        // Create new period for today
        var defaultHours = referenceHours ?? GetDefaultReferenceHours(periodType);
        return await CreatePeriodAsync(periodType, today, defaultHours);
    }

    public async Task<TimePeriod> CreatePeriodAsync(PeriodType periodType, DateTime startDate, decimal referenceHours)
    {
        TimePeriod period = periodType switch
        {
            PeriodType.WEEKLY => TimePeriod.CreateWeeklyPeriod(startDate, referenceHours),
            PeriodType.BIWEEKLY => TimePeriod.CreateBiweeklyPeriod(startDate, referenceHours),
            PeriodType.MONTHLY => TimePeriod.CreateMonthlyPeriod(startDate, referenceHours),
            _ => throw new ArgumentException($"Unsupported period type: {periodType}")
        };

        // Check for overlapping periods
        var overlapping = await _context.TimePeriods
            .Where(tp => tp.Type == periodType && tp.IsActive)
            .Where(tp => tp.OverlapsWith(period.StartDate, period.EndDate))
            .AnyAsync();

        if (overlapping)
        {
            throw new InvalidOperationException($"Period overlaps with existing {periodType} period");
        }

        _context.TimePeriods.Add(period);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new {PeriodType} period: {PeriodName} ({StartDate} to {EndDate})",
            periodType, period.Name, period.StartDate, period.EndDate);

        return period;
    }

    public async Task<IEnumerable<TimePeriod>> GeneratePeriodsForYearAsync(int year, PeriodType periodType, decimal referenceHours)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = new DateTime(year, 12, 31);
        var periods = new List<TimePeriod>();
        var currentDate = startOfYear;

        while (currentDate <= endOfYear)
        {
            try
            {
                // Check if period already exists for this date
                var existingPeriod = await FindPeriodForDateAsync(currentDate, periodType);
                if (existingPeriod != null)
                {
                    periods.Add(existingPeriod);
                    currentDate = existingPeriod.EndDate.AddDays(1);
                    continue;
                }

                var period = await CreatePeriodAsync(periodType, currentDate, referenceHours);
                periods.Add(period);
                currentDate = period.EndDate.AddDays(1);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Skipping period creation for {Date}: {Error}", currentDate, ex.Message);
                currentDate = GetNextPeriodStart(currentDate, periodType);
            }
        }

        _logger.LogInformation("Generated {Count} {PeriodType} periods for year {Year}",
            periods.Count, periodType, year);

        return periods;
    }

    public async Task<TimePeriod?> FindPeriodForDateAsync(DateTime date, PeriodType? periodType = null)
    {
        var query = _context.TimePeriods.Where(tp => tp.IsActive);

        if (periodType.HasValue)
        {
            query = query.Where(tp => tp.Type == periodType.Value);
        }

        return await query
            .FirstOrDefaultAsync(tp => tp.ContainsDate(date));
    }

    public async Task<IEnumerable<TimePeriod>> GetActivePeriodsAsync()
    {
        return await _context.TimePeriods
            .Where(tp => tp.IsActive)
            .OrderBy(tp => tp.Type)
            .ThenBy(tp => tp.StartDate)
            .ToListAsync();
    }

    public async Task<bool> SetCurrentPeriodAsync(Guid periodId)
    {
        var period = await _context.TimePeriods.FindAsync(periodId);
        if (period == null || !period.IsActive)
        {
            return false;
        }

        // Remove current flag from all periods of the same type
        var currentPeriods = await _context.TimePeriods
            .Where(tp => tp.Type == period.Type && tp.IsCurrent)
            .ToListAsync();

        foreach (var currentPeriod in currentPeriods)
        {
            currentPeriod.RemoveFromCurrent();
        }

        // Set new current period
        period.SetAsCurrent();
        await _context.SaveChangesAsync();

        _logger.LogInformation("Set period {PeriodId} ({PeriodName}) as current",
            periodId, period.Name);

        return true;
    }

    public async Task<bool> ValidateTimePeriodForDateAsync(DateTime date, Guid timePeriodId)
    {
        var period = await _context.TimePeriods.FindAsync(timePeriodId);
        return period != null && period.IsActive && period.ContainsDate(date);
    }

    public async Task<IEnumerable<TimePeriod>> GetUserAccessiblePeriodsAsync(Guid userId)
    {
        // Get user to determine their role and area
        var user = await _context.Users
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Enumerable.Empty<TimePeriod>();
        }

        // For now, return all active periods
        // TODO: Implement role-based filtering when user roles are clarified
        return await GetActivePeriodsAsync();
    }

    private static decimal GetDefaultReferenceHours(PeriodType periodType)
    {
        return periodType switch
        {
            PeriodType.WEEKLY => 40m,
            PeriodType.BIWEEKLY => 80m,
            PeriodType.MONTHLY => 160m,
            _ => 40m
        };
    }

    private static DateTime GetNextPeriodStart(DateTime currentDate, PeriodType periodType)
    {
        return periodType switch
        {
            PeriodType.WEEKLY => currentDate.AddDays(7),
            PeriodType.BIWEEKLY => currentDate.AddDays(14),
            PeriodType.MONTHLY => currentDate.AddMonths(1),
            _ => currentDate.AddDays(1)
        };
    }
}