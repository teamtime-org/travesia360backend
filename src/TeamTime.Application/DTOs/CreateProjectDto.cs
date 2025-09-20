using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AreaId { get; set; }
    public Priority Priority { get; set; } = Priority.MEDIUM;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public bool IsGeneral { get; set; } = false;
}