using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class TimePeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PeriodType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal ReferenceHours { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Additional calculated properties
    public int WorkingDays { get; set; }
    public decimal DailyReferenceHours { get; set; }
    public decimal WeeklyReferenceHours { get; set; }
    public string PeriodIdentifier { get; set; } = string.Empty;
    public bool IsCurrentPeriod { get; set; }

    // Statistics (optional, can be populated when needed)
    public decimal? TotalHoursLogged { get; set; }
    public int? TotalEntriesCount { get; set; }
    public decimal? CompletionPercentage { get; set; }
}