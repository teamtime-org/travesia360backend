using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Projects.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class GetProjectsByUserQueryHandler : IQueryHandler<GetProjectsByUserQuery, Result<PagedResult<ProjectDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Project, ProjectDto> _mapper;

    public GetProjectsByUserQueryHandler(IUnitOfWork unitOfWork, IMapper<Project, ProjectDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PagedResult<ProjectDto>>> HandleAsync(GetProjectsByUserQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null)
            {
                return Result<PagedResult<ProjectDto>>.Failure("User not found");
            }

            var projects = await _unitOfWork.Projects.GetByUserIdPagedAsync(
                query.UserId,
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Status,
                query.Priority,
                query.IsActive,
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
            return Result<PagedResult<ProjectDto>>.Failure($"Error retrieving user projects: {ex.Message}");
        }
    }
}