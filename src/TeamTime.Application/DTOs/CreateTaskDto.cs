using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class CreateTaskDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Priority Priority { get; set; } = Priority.MEDIUM;
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}