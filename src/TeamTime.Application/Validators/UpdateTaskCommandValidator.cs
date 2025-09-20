using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Validators;

namespace TeamTime.Application.Validators;

public class UpdateTaskCommandValidator : IValidator<UpdateTaskCommand>
{
    public ValidationResult Validate(UpdateTaskCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.Id), "Task ID is required"));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add(new ValidationError(nameof(command.Name), "Task name is required"));
        }
        else if (command.Name.Length > 200)
        {
            errors.Add(new ValidationError(nameof(command.Name), "Task name cannot exceed 200 characters"));
        }

        if (!string.IsNullOrEmpty(command.Description) && command.Description.Length > 1000)
        {
            errors.Add(new ValidationError(nameof(command.Description), "Task description cannot exceed 1000 characters"));
        }

        if (command.DueDate.HasValue && command.DueDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add(new ValidationError(nameof(command.DueDate), "Due date cannot be in the past"));
        }

        if (command.EstimatedHours.HasValue && command.EstimatedHours <= 0)
        {
            errors.Add(new ValidationError(nameof(command.EstimatedHours), "Estimated hours must be positive"));
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return Task.FromResult(result);
    }
}