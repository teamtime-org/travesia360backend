using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class TimePeriodSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PeriodType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal ReferenceHours { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
    public string PeriodIdentifier { get; set; } = string.Empty;
    public bool IsCurrentPeriod { get; set; }
}