using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public bool IsGeneral { get; set; }
    public bool IsActive { get; set; }
    public bool IsOverdue { get; set; }
    public int AssignedUserCount { get; set; }
    public int TaskCount { get; set; }
    public decimal TotalLoggedHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}