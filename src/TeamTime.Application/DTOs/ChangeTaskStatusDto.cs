using TaskStatus = TeamTime.Domain.Enums.TaskStatus;

namespace TeamTime.Application.DTOs;

public class ChangeTaskStatusDto
{
    public TaskStatus Status { get; set; }
}