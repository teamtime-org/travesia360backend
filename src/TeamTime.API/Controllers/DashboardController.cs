using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Application.Services;
using TeamTime.Domain.Enums;

namespace TeamTime.API.Controllers;

/// <summary>
/// Dashboard controller for system statistics and analytics
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class DashboardController : BaseApiController
{
    public DashboardController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Get dashboard summary with general system statistics
    /// </summary>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="areaId">Area ID for filtering (optional)</param>
    /// <param name="includeInactive">Include inactive items (optional)</param>
    /// <param name="topProjectsCount">Number of top projects to include (default: 5)</param>
    /// <param name="topUsersCount">Number of top users to include (default: 5)</param>
    /// <returns>Dashboard summary with system statistics</returns>
    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDashboardSummary(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? areaId = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int topProjectsCount = 5,
        [FromQuery] int topUsersCount = 5)
    {
        var query = new GetDashboardSummaryQuery
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            AreaId = areaId,
            IncludeInactive = includeInactive,
            TopProjectsCount = topProjectsCount,
            TopUsersCount = topUsersCount
        };

        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get statistics for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="includeProjectBreakdown">Include project breakdown (default: true)</param>
    /// <param name="includeComparisonPeriod">Include comparison with previous period (default: true)</param>
    /// <returns>User statistics</returns>
    [HttpGet("user/{userId:guid}/stats")]
    [ProducesResponseType(typeof(UserStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserStats(
        Guid userId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] bool includeProjectBreakdown = true,
        [FromQuery] bool includeComparisonPeriod = true)
    {
        // Authorization: Users can only view their own stats unless they are Admin/Manager
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        if (currentUserId != userId &&
            currentUserRole != UserRole.ADMINISTRADOR &&
            currentUserRole != UserRole.COORDINADOR)
        {
            return Forbid("You can only view your own statistics");
        }

        var query = new GetUserStatsQuery
        {
            UserId = userId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            IncludeProjectBreakdown = includeProjectBreakdown,
            IncludeComparisonPeriod = includeComparisonPeriod
        };

        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsFailure && result.Error == "User not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get statistics for a specific project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="includeTeamMembers">Include team members breakdown (default: true)</param>
    /// <param name="includeTaskBreakdown">Include task breakdown (default: true)</param>
    /// <returns>Project statistics</returns>
    [HttpGet("project/{projectId:guid}/stats")]
    [ProducesResponseType(typeof(ProjectStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectStats(
        Guid projectId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] bool includeTeamMembers = true,
        [FromQuery] bool includeTaskBreakdown = true)
    {
        var query = new GetProjectStatsQuery
        {
            ProjectId = projectId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            IncludeTeamMembers = includeTeamMembers,
            IncludeTaskBreakdown = includeTaskBreakdown
        };

        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsFailure && result.Error == "Project not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get time distribution by project and/or area
    /// </summary>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="userId">User ID for filtering (optional)</param>
    /// <param name="areaId">Area ID for filtering (optional)</param>
    /// <param name="groupBy">Group by 'project', 'area', or 'both' (default: 'project')</param>
    /// <param name="includeSubItems">Include sub-items (default: true)</param>
    /// <param name="maxItems">Maximum number of items to return (default: 10)</param>
    /// <returns>Time distribution summary</returns>
    [HttpGet("time-distribution")]
    [ProducesResponseType(typeof(TimeDistributionSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTimeDistribution(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] Guid? areaId = null,
        [FromQuery] string groupBy = "project",
        [FromQuery] bool includeSubItems = true,
        [FromQuery] int maxItems = 10)
    {
        // Authorization: Collaborators can only view their own time distribution
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        if (currentUserRole == UserRole.COLABORADOR && userId.HasValue && userId.Value != currentUserId)
        {
            return Forbid("Collaborators can only view their own time distribution");
        }

        // If no userId specified and user is collaborator, default to their own data
        if (!userId.HasValue && currentUserRole == UserRole.COLABORADOR)
        {
            userId = currentUserId;
        }

        if (!new[] { "project", "area", "both" }.Contains(groupBy.ToLower()))
        {
            return BadRequest("GroupBy must be 'project', 'area', or 'both'");
        }

        var query = new GetTimeDistributionQuery
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            UserId = userId,
            AreaId = areaId,
            GroupBy = groupBy.ToLower(),
            IncludeSubItems = includeSubItems,
            MaxItems = Math.Min(maxItems, 50) // Limit to prevent excessive data
        };

        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get weekly hours summary and trends
    /// </summary>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="userId">User ID for filtering (optional)</param>
    /// <param name="projectId">Project ID for filtering (optional)</param>
    /// <param name="areaId">Area ID for filtering (optional)</param>
    /// <param name="includeDailyBreakdown">Include daily breakdown (default: true)</param>
    /// <param name="includeProjectBreakdown">Include project breakdown (default: false)</param>
    /// <param name="weeklyTarget">Weekly hours target (default: 40)</param>
    /// <param name="weeksCount">Number of weeks to include (default: 12)</param>
    /// <returns>Weekly hours summary</returns>
    [HttpGet("weekly-hours")]
    [ProducesResponseType(typeof(WeeklyHoursSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetWeeklyHours(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] Guid? projectId = null,
        [FromQuery] Guid? areaId = null,
        [FromQuery] bool includeDailyBreakdown = true,
        [FromQuery] bool includeProjectBreakdown = false,
        [FromQuery] decimal weeklyTarget = 40,
        [FromQuery] int weeksCount = 12)
    {
        // Authorization: Collaborators can only view their own weekly hours
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        if (currentUserRole == UserRole.COLABORADOR && userId.HasValue && userId.Value != currentUserId)
        {
            return Forbid("Collaborators can only view their own weekly hours");
        }

        // If no userId specified and user is collaborator, default to their own data
        if (!userId.HasValue && currentUserRole == UserRole.COLABORADOR)
        {
            userId = currentUserId;
        }

        var query = new GetWeeklyHoursQuery
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            UserId = userId,
            ProjectId = projectId,
            AreaId = areaId,
            IncludeDailyBreakdown = includeDailyBreakdown,
            IncludeProjectBreakdown = includeProjectBreakdown,
            WeeklyTarget = Math.Max(weeklyTarget, 0),
            WeeksCount = Math.Min(Math.Max(weeksCount, 1), 52) // Limit between 1 and 52 weeks
        };

        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get collaborator-specific dashboard with personalized data
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="recentTasksCount">Number of recent tasks to include (default: 5)</param>
    /// <param name="recentTimeEntriesCount">Number of recent time entries to include (default: 10)</param>
    /// <param name="weeklyHoursTarget">Weekly hours target (default: 40)</param>
    /// <param name="includeWeeklyProgress">Include weekly progress (default: true)</param>
    /// <param name="includeTimeDistribution">Include time distribution (default: true)</param>
    /// <returns>Collaborator dashboard data</returns>
    [HttpGet("collaborator/{userId:guid}")]
    [ProducesResponseType(typeof(CollaboratorDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCollaboratorDashboard(
        Guid userId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int recentTasksCount = 5,
        [FromQuery] int recentTimeEntriesCount = 10,
        [FromQuery] decimal weeklyHoursTarget = 40,
        [FromQuery] bool includeWeeklyProgress = true,
        [FromQuery] bool includeTimeDistribution = true)
    {
        // Authorization: Users can only view their own dashboard unless they are Admin/Manager
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        if (currentUserId != userId &&
            currentUserRole != UserRole.ADMINISTRADOR &&
            currentUserRole != UserRole.COORDINADOR)
        {
            return Forbid("You can only view your own dashboard");
        }

        var query = new GetCollaboratorDashboardQuery
        {
            UserId = userId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            RecentTasksCount = Math.Min(Math.Max(recentTasksCount, 1), 20),
            RecentTimeEntriesCount = Math.Min(Math.Max(recentTimeEntriesCount, 1), 50),
            WeeklyHoursTarget = Math.Max(weeklyHoursTarget, 0),
            IncludeWeeklyProgress = includeWeeklyProgress,
            IncludeTimeDistribution = includeTimeDistribution
        };

        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsFailure && result.Error == "User not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get current user's dashboard (convenience endpoint)
    /// </summary>
    /// <param name="dateFrom">Start date for filtering (optional)</param>
    /// <param name="dateTo">End date for filtering (optional)</param>
    /// <param name="recentTasksCount">Number of recent tasks to include (default: 5)</param>
    /// <param name="recentTimeEntriesCount">Number of recent time entries to include (default: 10)</param>
    /// <param name="weeklyHoursTarget">Weekly hours target (default: 40)</param>
    /// <param name="includeWeeklyProgress">Include weekly progress (default: true)</param>
    /// <param name="includeTimeDistribution">Include time distribution (default: true)</param>
    /// <returns>Current user's dashboard data</returns>
    [HttpGet("my-dashboard")]
    [ProducesResponseType(typeof(CollaboratorDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyDashboard(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int recentTasksCount = 5,
        [FromQuery] int recentTimeEntriesCount = 10,
        [FromQuery] decimal weeklyHoursTarget = 40,
        [FromQuery] bool includeWeeklyProgress = true,
        [FromQuery] bool includeTimeDistribution = true)
    {
        var currentUserId = GetCurrentUserId();

        return await GetCollaboratorDashboard(
            currentUserId,
            dateFrom,
            dateTo,
            recentTasksCount,
            recentTimeEntriesCount,
            weeklyHoursTarget,
            includeWeeklyProgress,
            includeTimeDistribution);
    }

    #region Helper Methods

    /// <summary>
    /// Get the current user's ID from the JWT token
    /// </summary>
    /// <returns>Current user's ID</returns>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User ID not found in token");
    }

    /// <summary>
    /// Get the current user's role from the JWT token
    /// </summary>
    /// <returns>Current user's role</returns>
    private UserRole GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null && Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            return role;
        }

        return UserRole.COLABORADOR; // Default to most restrictive role
    }

    #endregion
}