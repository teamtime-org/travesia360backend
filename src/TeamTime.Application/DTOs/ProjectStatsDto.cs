using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class ProjectStatsDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BudgetHours { get; set; }
    public decimal HoursRemaining { get; set; }
    public decimal ProgressPercentage { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int ActiveUsers { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime LastActivity { get; set; }
    public List<UserStatsDto> TeamMembers { get; set; } = new();
    public decimal AverageHoursPerDay { get; set; }
    public bool IsActive { get; set; }
}