using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Dashboard.Handlers;

public class GetDashboardSummaryQueryHandler : IQueryHandler<GetDashboardSummaryQuery, Result<DashboardSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardSummaryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<DashboardSummaryDto>> HandleAsync(GetDashboardSummaryQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var dateFrom = DateOnly.FromDateTime(query.DateFrom ?? DateTime.Now.Date.AddDays(-30));
            var dateTo = DateOnly.FromDateTime(query.DateTo ?? DateTime.Now.Date);

            // Get time entries for the period
            var timeEntries = await _unitOfWork.TimeEntries.GetByDateRangeAsync(dateFrom, dateTo, cancellationToken);
            var totalHours = timeEntries.Sum(te => te.Hours);

            // Get active projects count
            var allProjects = await _unitOfWork.Projects.GetActiveProjectsAsync(cancellationToken);
            var activeProjects = allProjects.Count();

            // Get completed tasks count
            var allTasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);
            var completedTasks = allTasks.Count(t => t.Status == Domain.Enums.TaskStatus.DONE);

            // Get active users count
            var activeUsers = await _unitOfWork.Users.GetActiveUsersAsync(cancellationToken);
            var activeUsersCount = activeUsers.Count();

            // Get pending approvals count
            var pendingApprovals = await _unitOfWork.TimeEntries.GetPendingApprovalsAsync(cancellationToken);
            var pendingApprovalsCount = pendingApprovals.Count();

            // Calculate monthly hours
            var thisMonthStart = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            var thisMonthEnd = DateOnly.FromDateTime(thisMonthStart.ToDateTime(TimeOnly.MinValue).AddMonths(1).AddDays(-1));
            var thisMonthEntries = await _unitOfWork.TimeEntries.GetByDateRangeAsync(thisMonthStart, thisMonthEnd, cancellationToken);
            var thisMonthHours = thisMonthEntries.Sum(te => te.Hours);

            var lastMonthStart = DateOnly.FromDateTime(thisMonthStart.ToDateTime(TimeOnly.MinValue).AddMonths(-1));
            var lastMonthEnd = DateOnly.FromDateTime(thisMonthStart.ToDateTime(TimeOnly.MinValue).AddDays(-1));
            var lastMonthEntries = await _unitOfWork.TimeEntries.GetByDateRangeAsync(lastMonthStart, lastMonthEnd, cancellationToken);
            var lastMonthHours = lastMonthEntries.Sum(te => te.Hours);

            var monthlyChange = lastMonthHours > 0
                ? ((thisMonthHours - lastMonthHours) / lastMonthHours) * 100
                : 0;

            var result = new DashboardSummaryDto
            {
                TotalHoursWorked = totalHours,
                ActiveProjects = activeProjects,
                CompletedTasks = completedTasks,
                ActiveUsers = activeUsersCount,
                PendingApprovals = pendingApprovalsCount,
                TotalHoursThisMonth = thisMonthHours,
                TotalHoursLastMonth = lastMonthHours,
                MonthlyHoursChange = monthlyChange,
                TopProjects = new List<ProjectStatsDto>(),
                TopUsers = new List<UserStatsDto>(),
                DateFrom = dateFrom.ToDateTime(TimeOnly.MinValue),
                DateTo = dateTo.ToDateTime(TimeOnly.MinValue)
            };

            return Result<DashboardSummaryDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<DashboardSummaryDto>.Failure($"Error retrieving dashboard summary: {ex.Message}");
        }
    }
}