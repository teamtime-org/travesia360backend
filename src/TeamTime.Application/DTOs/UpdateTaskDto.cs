using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class UpdateTaskDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}