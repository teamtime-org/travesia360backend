using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Application.Validators;

namespace TeamTime.Application.Validators;

public class AssignUserToProjectCommandValidator : IValidator<AssignUserToProjectCommand>
{
    public ValidationResult Validate(AssignUserToProjectCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(AssignUserToProjectCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.ProjectId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.ProjectId), "Project ID is required"));
        }

        if (command.UserId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.UserId), "User ID is required"));
        }

        if (command.AssignedById == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.AssignedById), "Assigned by user ID is required"));
        }

        if (command.UserId == command.AssignedById)
        {
            errors.Add(new ValidationError(nameof(command.UserId), "User cannot assign themselves to a project"));
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return Task.FromResult(result);
    }
}