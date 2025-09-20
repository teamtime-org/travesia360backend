using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class CompleteTaskCommandHandler : ICommandHandler<CompleteTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TeamTimeTask, TaskDto> _mapper;

    public CompleteTaskCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TeamTimeTask, TaskDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<TaskDto>> HandleAsync(CompleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find task
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.Id, cancellationToken);
            if (task == null)
            {
                return Result<TaskDto>.Failure("Task not found");
            }

            // Complete task
            task.Complete();

            // Save changes
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Get with navigation properties for mapping
            var updatedTask = await _unitOfWork.Tasks.GetWithProjectAsync(task.Id, cancellationToken);

            var taskDto = _mapper.Map(updatedTask!);
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TaskDto>.Failure($"Failed to complete task: {ex.Message}");
        }
    }
}