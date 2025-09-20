using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TeamTimeTask, TaskDto> _mapper;
    private readonly IValidator<CreateTaskCommand> _validator;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TeamTimeTask, TaskDto> mapper,
        IValidator<CreateTaskCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<TaskDto>> HandleAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
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
            // Verify project exists
            var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
            if (project == null)
            {
                return Result<TaskDto>.Failure("Project not found");
            }

            // Create task
            var task = new TeamTimeTask(command.Name, command.ProjectId, command.Description);

            // Set optional properties
            if (command.Priority != Domain.Enums.Priority.MEDIUM)
            {
                task.UpdatePriority(command.Priority);
            }

            if (command.DueDate.HasValue)
            {
                task.UpdateDueDate(command.DueDate);
            }

            if (command.EstimatedHours.HasValue)
            {
                task.UpdateEstimatedHours(command.EstimatedHours);
            }

            // Add to repository
            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Get with navigation properties for mapping
            var createdTask = await _unitOfWork.Tasks.GetWithProjectAsync(task.Id, cancellationToken);

            var taskDto = _mapper.Map(createdTask!);
            return Result<TaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TaskDto>.Failure($"Failed to create task: {ex.Message}");
        }
    }
}