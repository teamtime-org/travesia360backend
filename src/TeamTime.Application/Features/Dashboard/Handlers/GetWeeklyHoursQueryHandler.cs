using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;
using System.Globalization;

namespace TeamTime.Application.Features.Dashboard.Handlers;

public class GetWeeklyHoursQueryHandler : IQueryHandler<GetWeeklyHoursQuery, Result<WeeklyHoursSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWeeklyHoursQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<WeeklyHoursSummaryDto>> HandleAsync(GetWeeklyHoursQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var dateTo = DateOnly.FromDateTime(query.DateTo ?? DateTime.Now.Date);
            var dateFrom = DateOnly.FromDateTime(query.DateFrom ?? dateTo.ToDateTime(TimeOnly.MinValue).AddDays(-7 * query.WeeksCount));

            // Get time entries for the period
            var timeEntries = query.UserId.HasValue
                ? await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(query.UserId.Value, dateFrom, dateTo, cancellationToken)
                : await _unitOfWork.TimeEntries.GetByDateRangeAsync(dateFrom, dateTo, cancellationToken);

            var weeks = new List<WeeklyHoursDto>();
            var totalHours = 0m;

            // Process each week
            var currentDate = GetStartOfWeek(dateFrom.ToDateTime(TimeOnly.MinValue));
            while (currentDate <= dateTo.ToDateTime(TimeOnly.MinValue))
            {
                var weekEnd = currentDate.AddDays(6);
                var weekEntries = timeEntries.Where(te =>
                    te.Date.ToDateTime(TimeOnly.MinValue) >= currentDate &&
                    te.Date.ToDateTime(TimeOnly.MinValue) <= weekEnd).ToList();
                var weekHours = weekEntries.Sum(te => te.Hours);

                var dailyBreakdown = new List<DailyHoursDto>();
                if (query.IncludeDailyBreakdown)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        var day = currentDate.AddDays(i);
                        var dayEntries = weekEntries.Where(te => te.Date.ToDateTime(TimeOnly.MinValue).Date == day.Date).ToList();
                        var dayHours = dayEntries.Sum(te => te.Hours);
                        var timeEntriesCount = dayEntries.Count;

                        dailyBreakdown.Add(new DailyHoursDto
                        {
                            Date = day,
                            DayOfWeek = day.DayOfWeek.ToString(),
                            Hours = dayHours,
                            TimeEntries = timeEntriesCount
                        });
                    }
                }

                var calendar = CultureInfo.InvariantCulture.Calendar;
                var weekNumber = calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                weeks.Add(new WeeklyHoursDto
                {
                    WeekStartDate = currentDate,
                    WeekEndDate = weekEnd,
                    TotalHours = weekHours,
                    WeekNumber = weekNumber,
                    Year = currentDate.Year,
                    DailyBreakdown = dailyBreakdown,
                    ProjectBreakdown = new List<ProjectStatsDto>()
                });

                totalHours += weekHours;
                currentDate = currentDate.AddDays(7);
            }

            // Calculate average weekly hours
            var averageWeeklyHours = weeks.Count > 0 ? totalHours / weeks.Count : 0;

            // Calculate target achievement percentage
            var targetAchievementPercentage = query.WeeklyTarget > 0
                ? (averageWeeklyHours / query.WeeklyTarget) * 100
                : 0;

            var result = new WeeklyHoursSummaryDto
            {
                Weeks = weeks,
                AverageWeeklyHours = averageWeeklyHours,
                TotalHours = totalHours,
                DateFrom = dateFrom.ToDateTime(TimeOnly.MinValue),
                DateTo = dateTo.ToDateTime(TimeOnly.MinValue),
                WeeklyTarget = query.WeeklyTarget,
                TargetAchievementPercentage = targetAchievementPercentage
            };

            return Result<WeeklyHoursSummaryDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<WeeklyHoursSummaryDto>.Failure($"Error retrieving weekly hours: {ex.Message}");
        }
    }

    private DateTime GetStartOfWeek(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}