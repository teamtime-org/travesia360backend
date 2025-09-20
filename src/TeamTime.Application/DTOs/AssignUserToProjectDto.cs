namespace TeamTime.Application.DTOs;

public class AssignUserToProjectDto
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid AssignedById { get; set; }
}