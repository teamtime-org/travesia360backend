using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Dashboard.Handlers;

public class GetCollaboratorDashboardQueryHandler : IQueryHandler<GetCollaboratorDashboardQuery, Result<CollaboratorDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCollaboratorDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<CollaboratorDashboardDto>> HandleAsync(GetCollaboratorDashboardQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null)
            {
                return Result<CollaboratorDashboardDto>.Failure("User not found");
            }

            var today = DateOnly.FromDateTime(DateTime.Now.Date);
            var weekStart = GetStartOfWeek(today);
            var weekEnd = weekStart.AddDays(6);
            var monthStart = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            var monthEnd = DateOnly.FromDateTime(monthStart.ToDateTime(TimeOnly.MinValue).AddMonths(1).AddDays(-1));

            var dateFrom = DateOnly.FromDateTime(query.DateFrom ?? monthStart.ToDateTime(TimeOnly.MinValue));
            var dateTo = DateOnly.FromDateTime(query.DateTo ?? monthEnd.ToDateTime(TimeOnly.MinValue));

            // Get time entries for different periods
            var todayEntries = await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(
                query.UserId, today, today, cancellationToken);
            var hoursToday = todayEntries.Sum(te => te.Hours);

            var weekEntries = await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(
                query.UserId, weekStart, weekEnd, cancellationToken);
            var hoursThisWeek = weekEntries.Sum(te => te.Hours);

            var monthEntries = await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(
                query.UserId, monthStart, monthEnd, cancellationToken);
            var hoursThisMonth = monthEntries.Sum(te => te.Hours);

            // Calculate average hours per day for the month
            var workingDaysThisMonth = GetWorkingDaysInMonth(monthStart.ToDateTime(TimeOnly.MinValue));
            var averageHoursPerDay = workingDaysThisMonth > 0 ? hoursThisMonth / workingDaysThisMonth : 0;

            // Get task statistics (simplified since we don't have AssignedTo)
            var allTasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);
            var activeTasks = allTasks.Count(t => t.Status != Domain.Enums.TaskStatus.DONE && t.IsActive);
            var completedTasksThisMonth = allTasks.Count(t => t.Status == Domain.Enums.TaskStatus.DONE);
            var overdueTasks = allTasks.Count(t => t.DueDate < today && t.Status != Domain.Enums.TaskStatus.DONE);

            // Get active projects count (simplified)
            var activeProjects = await _unitOfWork.Projects.GetActiveProjectsAsync(cancellationToken);
            var activeProjectsCount = activeProjects.Count();

            // Get time entry statistics
            var allUserTimeEntries = await _unitOfWork.TimeEntries.GetByUserIdAsync(query.UserId, cancellationToken);
            var pendingTimeEntries = allUserTimeEntries.Count(te => !te.IsApproved && te.IsActive);
            var approvedTimeEntries = allUserTimeEntries.Count(te => te.IsApproved &&
                                                                     te.Date >= monthStart && te.Date <= monthEnd);

            // Calculate target achievement percentage
            var targetAchievementPercentage = query.WeeklyHoursTarget > 0
                ? (hoursThisWeek / query.WeeklyHoursTarget) * 100
                : 0;

            var result = new CollaboratorDashboardDto
            {
                UserId = user.Id,
                UserName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                AreaName = user.Area?.Name ?? string.Empty,

                TotalHoursToday = hoursToday,
                TotalHoursThisWeek = hoursThisWeek,
                TotalHoursThisMonth = hoursThisMonth,
                AverageHoursPerDay = averageHoursPerDay,

                ActiveTasks = activeTasks,
                CompletedTasksThisMonth = completedTasksThisMonth,
                OverdueTasks = overdueTasks,
                RecentTasks = new List<TaskDto>(),

                ActiveProjects = activeProjectsCount,
                MyProjects = new List<ProjectStatsDto>(),

                PendingTimeEntries = pendingTimeEntries,
                ApprovedTimeEntries = approvedTimeEntries,
                RejectedTimeEntries = 0,
                RecentTimeEntries = new List<TimeEntryDto>(),

                WeeklyProgress = new WeeklyHoursSummaryDto(),
                TimeDistribution = new TimeDistributionSummaryDto(),

                WeeklyHoursTarget = query.WeeklyHoursTarget,
                TargetAchievementPercentage = targetAchievementPercentage,

                LastUpdated = DateTime.Now
            };

            return Result<CollaboratorDashboardDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<CollaboratorDashboardDto>.Failure($"Error retrieving collaborator dashboard: {ex.Message}");
        }
    }

    private DateOnly GetStartOfWeek(DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
        return DateOnly.FromDateTime(dateTime.AddDays(-1 * diff));
    }

    private int GetWorkingDaysInMonth(DateTime monthStart)
    {
        var monthEnd = monthStart.AddMonths(1);
        var workingDays = 0;
        var current = monthStart;

        while (current < monthEnd)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
            current = current.AddDays(1);
        }

        return workingDays;
    }
}