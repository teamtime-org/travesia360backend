using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Tasks.Queries;

public class GetOverdueTasksQuery : IQuery<Result<PagedResult<TaskDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? ProjectId { get; set; }
}