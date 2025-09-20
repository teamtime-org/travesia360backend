using TaskStatus = TeamTime.Domain.Enums.TaskStatus;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public bool IsActive { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsInProgress { get; set; }
    public decimal TotalLoggedHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}