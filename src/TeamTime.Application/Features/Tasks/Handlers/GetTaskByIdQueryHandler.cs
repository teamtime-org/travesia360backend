using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TeamTimeTask, TaskDto> _mapper;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork, IMapper<TeamTimeTask, TaskDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<TaskDto>> HandleAsync(GetTaskByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetWithProjectAsync(query.Id, cancellationToken);

            if (task == null)
            {
                return Result<TaskDto>.Failure("Task not found");
            }

            var taskDto = _mapper.Map(task);
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TaskDto>.Failure($"Error retrieving task: {ex.Message}");
        }
    }
}