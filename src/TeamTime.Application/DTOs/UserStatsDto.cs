namespace TeamTime.Application.DTOs;

public class UserStatsDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int CompletedTasks { get; set; }
    public int ActiveProjects { get; set; }
    public decimal AverageHoursPerDay { get; set; }
    public decimal HoursThisMonth { get; set; }
    public decimal HoursLastMonth { get; set; }
    public decimal MonthlyHoursChange { get; set; }
    public int PendingTimeEntries { get; set; }
    public int ApprovedTimeEntries { get; set; }
    public List<ProjectStatsDto> ProjectBreakdown { get; set; } = new();
    public DateTime LastActivity { get; set; }
    public bool IsActive { get; set; }
}