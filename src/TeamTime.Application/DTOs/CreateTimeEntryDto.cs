namespace TeamTime.Application.DTOs;

public class CreateTimeEntryDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? TaskId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
}