using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTime.Application.Services;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Services;

public class TimeEntryValidationService : ITimeEntryValidationService
{
    private readonly ApplicationDbContext _context;
    private readonly ITimePeriodService _timePeriodService;
    private readonly ILogger<TimeEntryValidationService> _logger;

    // Configuration constants - these could be moved to appsettings.json
    private const decimal MaxDailyHours = 24m;
    private const decimal MaxWeeklyHours = 80m;
    private const decimal StandardDailyHours = 8m;

    public TimeEntryValidationService(
        ApplicationDbContext context,
        ITimePeriodService timePeriodService,
        ILogger<TimeEntryValidationService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _timePeriodService = timePeriodService ?? throw new ArgumentNullException(nameof(timePeriodService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ValidationResult> ValidateTimeEntryAsync(Guid userId, Guid projectId, DateOnly date, decimal hours, Guid? timePeriodId = null)
    {
        var errors = new List<string>();

        // Basic validation
        if (hours <= 0)
        {
            errors.Add("Hours must be greater than zero");
        }

        if (hours > MaxDailyHours)
        {
            errors.Add($"Hours cannot exceed {MaxDailyHours} per day");
        }

        // Validate date is not in the future
        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add("Cannot create time entries for future dates");
        }

        // Validate user exists and is active
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive)
        {
            errors.Add("User not found or inactive");
        }

        // Validate project exists and is active
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null || !project.IsActive)
        {
            errors.Add("Project not found or inactive");
        }

        TimePeriod? validPeriod = null;

        // Validate or determine time period
        if (timePeriodId.HasValue)
        {
            var specifiedPeriod = await _context.TimePeriods.FindAsync(timePeriodId.Value);
            if (specifiedPeriod == null || !specifiedPeriod.IsActive)
            {
                errors.Add("Specified time period not found or inactive");
            }
            else if (!specifiedPeriod.ContainsDate(date.ToDateTime(TimeOnly.MinValue)))
            {
                errors.Add($"Date {date} is not within the specified time period ({specifiedPeriod.StartDate:yyyy-MM-dd} to {specifiedPeriod.EndDate:yyyy-MM-dd})");
            }
            else
            {
                validPeriod = specifiedPeriod;
            }
        }
        else
        {
            // Find appropriate time period for the date
            try
            {
                validPeriod = await GetValidTimePeriodForDateAsync(date);
            }
            catch (InvalidOperationException ex)
            {
                errors.Add($"No valid time period found for date {date}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return ValidationResult.Failure(errors);
        }

        // Additional validations that require valid period
        if (validPeriod != null)
        {
            // Check daily hours limit
            if (await WouldExceedDailyLimitAsync(userId, date, hours))
            {
                var currentDaily = await GetTotalHoursForUserOnDateAsync(userId, date);
                errors.Add($"Adding {hours} hours would exceed daily limit. Current: {currentDaily} hours, Limit: {MaxDailyHours} hours");
            }

            // Check period hours limit (if applicable)
            if (await WouldExceedPeriodLimitAsync(userId, validPeriod.Id, hours))
            {
                var currentPeriod = await GetTotalHoursForUserInPeriodAsync(userId, validPeriod.Id);
                var limit = validPeriod.ReferenceHours * 1.5m; // Allow 150% of reference hours
                errors.Add($"Adding {hours} hours would exceed period limit. Current: {currentPeriod} hours, Limit: {limit} hours");
            }

            // Check user access to period
            if (!await CanUserCreateEntryInPeriodAsync(userId, validPeriod.Id))
            {
                errors.Add("User does not have permission to create entries in this time period");
            }
        }

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success(validPeriod);
    }

    public async Task<ValidationResult> ValidateTimeEntryUpdateAsync(Guid timeEntryId, decimal? hours = null, DateOnly? date = null, Guid? timePeriodId = null)
    {
        var timeEntry = await _context.TimeEntries
            .Include(te => te.TimePeriod)
            .FirstOrDefaultAsync(te => te.Id == timeEntryId);

        if (timeEntry == null)
        {
            return ValidationResult.Failure("Time entry not found");
        }

        if (timeEntry.IsApproved)
        {
            return ValidationResult.Failure("Cannot modify approved time entry");
        }

        if (!timeEntry.IsActive)
        {
            return ValidationResult.Failure("Cannot modify inactive time entry");
        }

        // Use current values if not provided
        var newHours = hours ?? timeEntry.Hours;
        var newDate = date ?? timeEntry.Date;
        var newTimePeriodId = timePeriodId ?? timeEntry.TimePeriodId;

        // Validate the updated entry
        return await ValidateTimeEntryAsync(timeEntry.UserId, timeEntry.ProjectId, newDate, newHours, newTimePeriodId);
    }

    public async Task<TimePeriod> GetValidTimePeriodForDateAsync(DateOnly date, PeriodType? preferredType = null)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        // Try to find existing period for the date
        var existingPeriod = await _timePeriodService.FindPeriodForDateAsync(dateTime, preferredType);
        if (existingPeriod != null)
        {
            return existingPeriod;
        }

        // If no existing period and we have a preferred type, try to create one
        if (preferredType.HasValue)
        {
            try
            {
                return await _timePeriodService.CreatePeriodAsync(preferredType.Value, dateTime,
                    GetDefaultReferenceHours(preferredType.Value));
            }
            catch (InvalidOperationException)
            {
                // Creation failed, continue to try other types
            }
        }

        // Try each period type in order of preference
        var periodTypes = new[] { PeriodType.WEEKLY, PeriodType.MONTHLY, PeriodType.BIWEEKLY };

        foreach (var type in periodTypes)
        {
            try
            {
                return await _timePeriodService.CreatePeriodAsync(type, dateTime, GetDefaultReferenceHours(type));
            }
            catch (InvalidOperationException)
            {
                // Try next type
                continue;
            }
        }

        throw new InvalidOperationException($"Cannot create any time period for date {date}");
    }

    public async Task<bool> CanUserCreateEntryInPeriodAsync(Guid userId, Guid timePeriodId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive)
        {
            return false;
        }

        var period = await _context.TimePeriods.FindAsync(timePeriodId);
        if (period == null || !period.IsActive)
        {
            return false;
        }

        // For now, all active users can create entries in active periods
        // TODO: Add more sophisticated role-based rules when requirements are clarified
        return true;
    }

    public async Task<decimal> GetTotalHoursForUserInPeriodAsync(Guid userId, Guid timePeriodId)
    {
        return await _context.TimeEntries
            .Where(te => te.UserId == userId && te.TimePeriodId == timePeriodId && te.IsActive)
            .SumAsync(te => te.Hours);
    }

    public async Task<bool> WouldExceedDailyLimitAsync(Guid userId, DateOnly date, decimal additionalHours, Guid? excludeTimeEntryId = null)
    {
        var currentHours = await GetTotalHoursForUserOnDateAsync(userId, date, excludeTimeEntryId);
        return (currentHours + additionalHours) > MaxDailyHours;
    }

    public async Task<bool> WouldExceedPeriodLimitAsync(Guid userId, Guid timePeriodId, decimal additionalHours, Guid? excludeTimeEntryId = null)
    {
        var period = await _context.TimePeriods.FindAsync(timePeriodId);
        if (period == null)
        {
            return false;
        }

        var currentHours = await _context.TimeEntries
            .Where(te => te.UserId == userId &&
                        te.TimePeriodId == timePeriodId &&
                        te.IsActive &&
                        (excludeTimeEntryId == null || te.Id != excludeTimeEntryId))
            .SumAsync(te => te.Hours);

        // Allow 150% of reference hours as a soft limit
        var limit = period.ReferenceHours * 1.5m;
        return (currentHours + additionalHours) > limit;
    }

    private async Task<decimal> GetTotalHoursForUserOnDateAsync(Guid userId, DateOnly date, Guid? excludeTimeEntryId = null)
    {
        return await _context.TimeEntries
            .Where(te => te.UserId == userId &&
                        te.Date == date &&
                        te.IsActive &&
                        (excludeTimeEntryId == null || te.Id != excludeTimeEntryId))
            .SumAsync(te => te.Hours);
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
}