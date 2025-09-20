using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Tasks.Commands;

public class StartTaskCommand : ICommand<Result<TaskDto>>
{
    public Guid Id { get; set; }
}