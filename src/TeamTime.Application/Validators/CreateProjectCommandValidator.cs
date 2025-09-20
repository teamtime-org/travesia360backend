using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Application.Validators;

namespace TeamTime.Application.Validators;

public class CreateProjectCommandValidator : IValidator<CreateProjectCommand>
{
    public ValidationResult Validate(CreateProjectCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add(new ValidationError(nameof(command.Name), "Project name is required"));
        }
        else if (command.Name.Length > 200)
        {
            errors.Add(new ValidationError(nameof(command.Name), "Project name cannot exceed 200 characters"));
        }

        if (!string.IsNullOrEmpty(command.Description) && command.Description.Length > 1000)
        {
            errors.Add(new ValidationError(nameof(command.Description), "Project description cannot exceed 1000 characters"));
        }

        if (command.AreaId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.AreaId), "Area ID is required"));
        }

        if (command.CreatedById == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.CreatedById), "Created by user ID is required"));
        }

        if (command.StartDate.HasValue && command.EndDate.HasValue && command.StartDate > command.EndDate)
        {
            errors.Add(new ValidationError(nameof(command.EndDate), "End date cannot be earlier than start date"));
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