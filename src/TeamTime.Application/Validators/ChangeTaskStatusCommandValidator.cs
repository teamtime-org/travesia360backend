using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Validators;
using TaskStatus = TeamTime.Domain.Enums.TaskStatus;

namespace TeamTime.Application.Validators;

public class ChangeTaskStatusCommandValidator : IValidator<ChangeTaskStatusCommand>
{
    public ValidationResult Validate(ChangeTaskStatusCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(ChangeTaskStatusCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.Id), "Task ID is required"));
        }

        if (!Enum.IsDefined(typeof(TaskStatus), command.Status))
        {
            errors.Add(new ValidationError(nameof(command.Status), "Invalid task status"));
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return Task.FromResult(result);
    }
}