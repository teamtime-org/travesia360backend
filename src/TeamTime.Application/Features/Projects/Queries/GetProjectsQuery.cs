using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Projects.Queries;

public class GetProjectsQuery : IQuery<Result<PagedResult<ProjectDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public ProjectStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public Guid? AreaId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsGeneral { get; set; }
    public bool? IsOverdue { get; set; }
}