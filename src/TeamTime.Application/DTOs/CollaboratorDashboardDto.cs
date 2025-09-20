using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class CollaboratorDashboardDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string AreaName { get; set; } = string.Empty;

    // Personal Statistics
    public decimal TotalHoursToday { get; set; }
    public decimal TotalHoursThisWeek { get; set; }
    public decimal TotalHoursThisMonth { get; set; }
    public decimal AverageHoursPerDay { get; set; }

    // Task Statistics
    public int ActiveTasks { get; set; }
    public int CompletedTasksThisMonth { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskDto> RecentTasks { get; set; } = new();

    // Project Statistics
    public int ActiveProjects { get; set; }
    public List<ProjectStatsDto> MyProjects { get; set; } = new();

    // Time Entry Statistics
    public int PendingTimeEntries { get; set; }
    public int ApprovedTimeEntries { get; set; }
    public int RejectedTimeEntries { get; set; }
    public List<TimeEntryDto> RecentTimeEntries { get; set; } = new();

    // Weekly Progress
    public WeeklyHoursSummaryDto WeeklyProgress { get; set; } = new();

    // Time Distribution
    public TimeDistributionSummaryDto TimeDistribution { get; set; } = new();

    // Goals and Targets
    public decimal WeeklyHoursTarget { get; set; }
    public decimal TargetAchievementPercentage { get; set; }

    public DateTime LastUpdated { get; set; }
}