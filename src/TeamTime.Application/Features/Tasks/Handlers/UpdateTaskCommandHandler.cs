using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TeamTimeTask, TaskDto> _mapper;
    private readonly IValidator<UpdateTaskCommand> _validator;

    public UpdateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TeamTimeTask, TaskDto> mapper,
        IValidator<UpdateTaskCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<TaskDto>> HandleAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<TaskDto>.Failure(errors);
        }

        try
        {
            // Find task
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.Id, cancellationToken);
            if (task == null)
            {
                return Result<TaskDto>.Failure("Task not found");
            }

            // Update task properties
            task.UpdateDetails(command.Name, command.Description);
            task.UpdatePriority(command.Priority);
            task.UpdateDueDate(command.DueDate);
            task.UpdateEstimatedHours(command.EstimatedHours);

            // Save changes
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Get with navigation properties for mapping
            var updatedTask = await _unitOfWork.Tasks.GetWithProjectAsync(task.Id, cancellationToken);

            var taskDto = _mapper.Map(updatedTask!);
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TaskDto>.Failure($"Failed to update task: {ex.Message}");
        }
    }
}