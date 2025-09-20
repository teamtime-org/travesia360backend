namespace TeamTime.Application.DTOs;

public class TimeEntryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? TaskId { get; set; }
    public string? TaskName { get; set; }
    public DateOnly Date { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}