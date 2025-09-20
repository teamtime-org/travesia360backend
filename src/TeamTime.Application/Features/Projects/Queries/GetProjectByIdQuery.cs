using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Projects.Queries;

public class GetProjectByIdQuery : IQuery<Result<ProjectDto?>>
{
    public Guid Id { get; set; }
}