namespace TeamTime.Application.DTOs;

public class WeeklyHoursDto
{
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public decimal TotalHours { get; set; }
    public int WeekNumber { get; set; }
    public int Year { get; set; }
    public List<DailyHoursDto> DailyBreakdown { get; set; } = new();
    public List<ProjectStatsDto> ProjectBreakdown { get; set; } = new();
}

public class DailyHoursDto
{
    public DateTime Date { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public int TimeEntries { get; set; }
}

public class WeeklyHoursSummaryDto
{
    public List<WeeklyHoursDto> Weeks { get; set; } = new();
    public decimal AverageWeeklyHours { get; set; }
    public decimal TotalHours { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public decimal WeeklyTarget { get; set; }
    public decimal TargetAchievementPercentage { get; set; }
}