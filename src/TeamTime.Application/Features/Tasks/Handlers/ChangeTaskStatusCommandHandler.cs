using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class ChangeTaskStatusCommandHandler : ICommandHandler<ChangeTaskStatusCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TeamTimeTask, TaskDto> _mapper;
    private readonly IValidator<ChangeTaskStatusCommand> _validator;

    public ChangeTaskStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TeamTimeTask, TaskDto> mapper,
        IValidator<ChangeTaskStatusCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<TaskDto>> HandleAsync(ChangeTaskStatusCommand command, CancellationToken cancellationToken = default)
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

            // Update status
            task.UpdateStatus(command.Status);

            // Save changes
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Get with navigation properties for mapping
            var updatedTask = await _unitOfWork.Tasks.GetWithProjectAsync(task.Id, cancellationToken);

            var taskDto = _mapper.Map(updatedTask!);
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TaskDto>.Failure($"Failed to change task status: {ex.Message}");
        }
    }
}