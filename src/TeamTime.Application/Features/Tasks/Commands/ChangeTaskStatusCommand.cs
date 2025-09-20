using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TaskStatus = TeamTime.Domain.Enums.TaskStatus;

namespace TeamTime.Application.Features.Tasks.Commands;

public class ChangeTaskStatusCommand : ICommand<Result<TaskDto>>
{
    public Guid Id { get; set; }
    public TaskStatus Status { get; set; }
}