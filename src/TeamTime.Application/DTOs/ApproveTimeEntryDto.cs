namespace TeamTime.Application.DTOs;

public class ApproveTimeEntryDto
{
    public Guid Id { get; set; }
    public Guid ApprovedById { get; set; }
}