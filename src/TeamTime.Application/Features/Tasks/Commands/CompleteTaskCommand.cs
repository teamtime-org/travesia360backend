using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Tasks.Commands;

public class CompleteTaskCommand : ICommand<Result<TaskDto>>
{
    public Guid Id { get; set; }
}