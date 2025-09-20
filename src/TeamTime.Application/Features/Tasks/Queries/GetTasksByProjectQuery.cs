using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TaskStatus = TeamTime.Domain.Enums.TaskStatus;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Tasks.Queries;

public class GetTasksByProjectQuery : IQuery<Result<PagedResult<TaskDto>>>
{
    public Guid ProjectId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public TaskStatus? Status { get; set; }
    public Priority? Priority { get; set; }
}