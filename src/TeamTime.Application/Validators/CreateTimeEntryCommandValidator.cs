using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Validators;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Validators;

public class CreateTimeEntryCommandValidator : IValidator<CreateTimeEntryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTimeEntryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public ValidationResult Validate(CreateTimeEntryCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public async Task<ValidationResult> ValidateAsync(CreateTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.UserId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.UserId), "User ID is required"));
        }

        if (command.ProjectId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.ProjectId), "Project ID is required"));
        }

        if (command.Hours < 0.25m)
        {
            errors.Add(new ValidationError(nameof(command.Hours), "Hours must be at least 0.25 (15 minutes)"));
        }

        if (command.Hours > 24)
        {
            errors.Add(new ValidationError(nameof(command.Hours), "Hours cannot exceed 24 per day"));
        }

        if (command.Date > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add(new ValidationError(nameof(command.Date), "Date cannot be in the future"));
        }

        // Check if user exists
        if (command.UserId != Guid.Empty)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                errors.Add(new ValidationError(nameof(command.UserId), "User not found"));
            }
        }

        // Check if project exists
        if (command.ProjectId != Guid.Empty)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
            if (project == null)
            {
                errors.Add(new ValidationError(nameof(command.ProjectId), "Project not found"));
            }
        }

        // Check if task exists (if provided)
        if (command.TaskId.HasValue)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.TaskId.Value, cancellationToken);
            if (task == null)
            {
                errors.Add(new ValidationError(nameof(command.TaskId), "Task not found"));
            }
        }

        // Check if user is assigned to project
        if (command.UserId != Guid.Empty && command.ProjectId != Guid.Empty)
        {
            var isAssigned = await _unitOfWork.TimeEntries.UserIsAssignedToProjectAsync(command.UserId, command.ProjectId, cancellationToken);
            if (!isAssigned)
            {
                errors.Add(new ValidationError("UserProject", "User is not assigned to this project"));
            }
        }

        // Check daily hours limit
        if (command.UserId != Guid.Empty && command.Hours > 0)
        {
            var dailyHours = await _unitOfWork.TimeEntries.GetTotalHoursByUserAndDateAsync(command.UserId, command.Date, cancellationToken);
            if (dailyHours + command.Hours > 24)
            {
                errors.Add(new ValidationError(nameof(command.Hours), $"Total daily hours would exceed 24. Current: {dailyHours}, Attempting to add: {command.Hours}"));
            }
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return result;
    }
}