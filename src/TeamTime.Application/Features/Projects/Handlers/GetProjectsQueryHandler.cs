using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Projects.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class GetProjectsQueryHandler : IQueryHandler<GetProjectsQuery, Result<PagedResult<ProjectDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Project, ProjectDto> _mapper;

    public GetProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper<Project, ProjectDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PagedResult<ProjectDto>>> HandleAsync(GetProjectsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Status,
                query.Priority,
                query.AreaId,
                query.IsActive,
                query.IsGeneral,
                query.IsOverdue,
                cancellationToken);

            var projectDtos = projects.Items.Select(_mapper.Map).ToList();

            var pagedResult = PagedResult<ProjectDto>.Create(
                projectDtos,
                projects.TotalCount,
                projects.PageNumber,
                projects.PageSize);

            return Result<PagedResult<ProjectDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProjectDto>>.Failure($"Error retrieving projects: {ex.Message}");
        }
    }
}