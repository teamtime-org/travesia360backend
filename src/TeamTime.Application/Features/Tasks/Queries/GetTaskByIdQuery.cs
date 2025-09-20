using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Tasks.Queries;

public class GetTaskByIdQuery : IQuery<Result<TaskDto>>
{
    public Guid Id { get; set; }
}