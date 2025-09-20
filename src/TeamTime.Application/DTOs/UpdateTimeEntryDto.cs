namespace TeamTime.Application.DTOs;

public class UpdateTimeEntryDto
{
    public Guid Id { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public Guid? TaskId { get; set; }
}