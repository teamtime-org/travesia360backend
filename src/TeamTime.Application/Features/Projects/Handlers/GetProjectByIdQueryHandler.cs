using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Projects.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class GetProjectByIdQueryHandler : IQueryHandler<GetProjectByIdQuery, Result<ProjectDto?>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Project, ProjectDto> _mapper;

    public GetProjectByIdQueryHandler(IUnitOfWork unitOfWork, IMapper<Project, ProjectDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<ProjectDto?>> HandleAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetWithAssignmentsAsync(query.Id, cancellationToken);

            if (project == null)
            {
                return Result<ProjectDto?>.Success(null);
            }

            var projectDto = _mapper.Map(project);
            return Result<ProjectDto?>.Success(projectDto);
        }
        catch (Exception ex)
        {
            return Result<ProjectDto?>.Failure($"Error retrieving project: {ex.Message}");
        }
    }
}