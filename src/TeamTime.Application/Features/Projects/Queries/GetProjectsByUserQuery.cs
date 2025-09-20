using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Projects.Queries;

public class GetProjectsByUserQuery : IQuery<Result<PagedResult<ProjectDto>>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public ProjectStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public bool? IsActive { get; set; }
}