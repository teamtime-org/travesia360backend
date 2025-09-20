using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Dashboard.Handlers;

public class GetUserStatsQueryHandler : IQueryHandler<GetUserStatsQuery, Result<UserStatsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<UserStatsDto>> HandleAsync(GetUserStatsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null)
            {
                return Result<UserStatsDto>.Failure("User not found");
            }

            var dateFrom = DateOnly.FromDateTime(query.DateFrom ?? DateTime.Now.Date.AddDays(-30));
            var dateTo = DateOnly.FromDateTime(query.DateTo ?? DateTime.Now.Date);

            // Get time entries for user in period
            var userTimeEntries = await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(
                query.UserId, dateFrom, dateTo, cancellationToken);
            var totalHours = userTimeEntries.Sum(te => te.Hours);

            // Get completed tasks count (simplified - no AssignedTo property in entity)
            var allTasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);
            var completedTasks = allTasks.Count(t => t.Status == Domain.Enums.TaskStatus.DONE);

            // Get active projects count (simplified - use all active projects)
            var activeProjects = await _unitOfWork.Projects.GetActiveProjectsAsync(cancellationToken);
            var activeProjectsCount = activeProjects.Count();

            // Calculate average hours per day
            var daysDiff = (dateTo.ToDateTime(TimeOnly.MinValue) - dateFrom.ToDateTime(TimeOnly.MinValue)).Days;
            var averageHoursPerDay = daysDiff > 0 ? totalHours / daysDiff : 0;

            // Get current month and last month hours
            var thisMonthStart = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            var thisMonthEnd = DateOnly.FromDateTime(thisMonthStart.ToDateTime(TimeOnly.MinValue).AddMonths(1).AddDays(-1));
            var thisMonthEntries = await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(
                query.UserId, thisMonthStart, thisMonthEnd, cancellationToken);
            var thisMonthHours = thisMonthEntries.Sum(te => te.Hours);

            var lastMonthStart = DateOnly.FromDateTime(thisMonthStart.ToDateTime(TimeOnly.MinValue).AddMonths(-1));
            var lastMonthEnd = DateOnly.FromDateTime(thisMonthStart.ToDateTime(TimeOnly.MinValue).AddDays(-1));
            var lastMonthEntries = await _unitOfWork.TimeEntries.GetByUserAndDateRangeAsync(
                query.UserId, lastMonthStart, lastMonthEnd, cancellationToken);
            var lastMonthHours = lastMonthEntries.Sum(te => te.Hours);

            // Calculate monthly change
            var monthlyChange = lastMonthHours > 0
                ? ((thisMonthHours - lastMonthHours) / lastMonthHours) * 100
                : 0;

            // Get pending and approved time entries
            var allUserTimeEntries = await _unitOfWork.TimeEntries.GetByUserIdAsync(query.UserId, cancellationToken);
            var pendingTimeEntries = allUserTimeEntries.Count(te => !te.IsApproved && te.IsActive);
            var approvedTimeEntries = allUserTimeEntries.Count(te => te.IsApproved &&
                                                                     te.Date >= dateFrom && te.Date <= dateTo);

            // Get last activity (simplified - use latest time entry)
            var lastTimeEntry = allUserTimeEntries.OrderByDescending(te => te.Date).FirstOrDefault();
            var lastActivity = lastTimeEntry?.Date.ToDateTime(TimeOnly.MinValue);

            var result = new UserStatsDto
            {
                UserId = user.Id,
                UserName = user.FullName,
                Email = user.Email,
                AreaName = user.Area?.Name ?? string.Empty,
                TotalHours = totalHours,
                CompletedTasks = completedTasks,
                ActiveProjects = activeProjectsCount,
                AverageHoursPerDay = averageHoursPerDay,
                HoursThisMonth = thisMonthHours,
                HoursLastMonth = lastMonthHours,
                MonthlyHoursChange = monthlyChange,
                PendingTimeEntries = pendingTimeEntries,
                ApprovedTimeEntries = approvedTimeEntries,
                ProjectBreakdown = new List<ProjectStatsDto>(),
                LastActivity = lastActivity ?? DateTime.MinValue,
                IsActive = user.IsActive
            };

            return Result<UserStatsDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<UserStatsDto>.Failure($"Error retrieving user statistics: {ex.Message}");
        }
    }
}