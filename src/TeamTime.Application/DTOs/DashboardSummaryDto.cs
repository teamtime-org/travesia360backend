namespace TeamTime.Application.DTOs;

public class DashboardSummaryDto
{
    public decimal TotalHoursWorked { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedTasks { get; set; }
    public int ActiveUsers { get; set; }
    public int PendingApprovals { get; set; }
    public decimal TotalHoursThisMonth { get; set; }
    public decimal TotalHoursLastMonth { get; set; }
    public decimal MonthlyHoursChange { get; set; }
    public List<ProjectStatsDto> TopProjects { get; set; } = new();
    public List<UserStatsDto> TopUsers { get; set; } = new();
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}