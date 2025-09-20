using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TeamTimeTask, TaskDto> _mapper;

    public GetTasksQueryHandler(IUnitOfWork unitOfWork, IMapper<TeamTimeTask, TaskDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PagedResult<TaskDto>>> HandleAsync(GetTasksQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedResult = await _unitOfWork.Tasks.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Status,
                query.Priority,
                query.ProjectId,
                query.IsActive,
                query.IsOverdue,
                cancellationToken);

            var taskDtos = _mapper.MapList(pagedResult.Items).ToList();

            var result = PagedResult<TaskDto>.Create(
                taskDtos,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize);

            return Result<PagedResult<TaskDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<TaskDto>>.Failure($"Error retrieving tasks: {ex.Message}");
        }
    }
}