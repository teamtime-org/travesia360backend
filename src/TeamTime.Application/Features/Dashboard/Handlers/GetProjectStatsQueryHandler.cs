using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Dashboard.Handlers;

public class GetProjectStatsQueryHandler : IQueryHandler<GetProjectStatsQuery, Result<ProjectStatsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<ProjectStatsDto>> HandleAsync(GetProjectStatsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get project
            var project = await _unitOfWork.Projects.GetByIdAsync(query.ProjectId, cancellationToken);
            if (project == null)
            {
                return Result<ProjectStatsDto>.Failure("Project not found");
            }

            var dateFrom = DateOnly.FromDateTime(query.DateFrom ?? project.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(-30));
            var dateTo = DateOnly.FromDateTime(query.DateTo ?? DateTime.Now.Date);

            // Get time entries for project in period
            var projectTimeEntries = await _unitOfWork.TimeEntries.GetByProjectAndDateRangeAsync(
                query.ProjectId, dateFrom, dateTo, cancellationToken);
            var totalHours = projectTimeEntries.Sum(te => te.Hours);

            // Calculate remaining hours using EstimatedHours (not BudgetHours)
            var estimatedHours = project.EstimatedHours ?? 0;
            var hoursRemaining = estimatedHours - totalHours;
            var progressPercentage = estimatedHours > 0 ? (totalHours / estimatedHours) * 100 : 0;

            // Get task statistics
            var allTasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);
            var projectTasks = allTasks.Where(t => t.ProjectId == query.ProjectId);
            var totalTasks = projectTasks.Count();
            var completedTasks = projectTasks.Count(t => t.Status == Domain.Enums.TaskStatus.DONE);

            // Get active users count (simplified - get all time entries and count unique users)
            var allProjectTimeEntries = await _unitOfWork.TimeEntries.GetByProjectIdAsync(query.ProjectId, cancellationToken);
            var activeUsers = allProjectTimeEntries.Select(te => te.UserId).Distinct().Count();

            // Get last activity (simplified - use latest time entry)
            var lastTimeEntry = allProjectTimeEntries.OrderByDescending(te => te.Date).FirstOrDefault();
            var lastActivity = lastTimeEntry?.Date.ToDateTime(TimeOnly.MinValue);

            // Calculate average hours per day
            var daysDiff = (dateTo.ToDateTime(TimeOnly.MinValue) - dateFrom.ToDateTime(TimeOnly.MinValue)).Days;
            var averageHoursPerDay = daysDiff > 0 ? totalHours / daysDiff : 0;

            var result = new ProjectStatsDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Description = project.Description,
                AreaName = project.Area?.Name ?? string.Empty,
                Status = project.Status,
                TotalHours = totalHours,
                BudgetHours = estimatedHours, // Use EstimatedHours as BudgetHours
                HoursRemaining = hoursRemaining,
                ProgressPercentage = progressPercentage,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                ActiveUsers = activeUsers,
                StartDate = project.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                EndDate = project.EndDate?.ToDateTime(TimeOnly.MinValue),
                LastActivity = lastActivity ?? DateTime.MinValue,
                TeamMembers = new List<UserStatsDto>(),
                AverageHoursPerDay = averageHoursPerDay,
                IsActive = project.IsActive
            };

            return Result<ProjectStatsDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ProjectStatsDto>.Failure($"Error retrieving project statistics: {ex.Message}");
        }
    }
}