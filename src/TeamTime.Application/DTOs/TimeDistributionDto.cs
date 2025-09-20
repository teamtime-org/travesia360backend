namespace TeamTime.Application.DTOs;

public class TimeDistributionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Project" or "Area"
    public decimal Hours { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
    public List<TimeDistributionDto> SubItems { get; set; } = new();
}

public class TimeDistributionSummaryDto
{
    public decimal TotalHours { get; set; }
    public List<TimeDistributionDto> ByProject { get; set; } = new();
    public List<TimeDistributionDto> ByArea { get; set; } = new();
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}