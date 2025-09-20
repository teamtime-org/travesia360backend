using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class UpdateProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}