using TeamTime.Application.Features.Areas.Commands;
using TeamTime.Application.Validators;

namespace TeamTime.Application.Validators;

public class UpdateAreaCommandValidator : IValidator<UpdateAreaCommand>
{
    public ValidationResult Validate(UpdateAreaCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(UpdateAreaCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.Id), "Area ID is required"));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add(new ValidationError(nameof(command.Name), "Area name is required"));
        }
        else if (command.Name.Length > 100)
        {
            errors.Add(new ValidationError(nameof(command.Name), "Area name cannot exceed 100 characters"));
        }

        if (!string.IsNullOrEmpty(command.Description) && command.Description.Length > 500)
        {
            errors.Add(new ValidationError(nameof(command.Description), "Description cannot exceed 500 characters"));
        }

        if (!string.IsNullOrEmpty(command.Color) && !IsValidHexColor(command.Color))
        {
            errors.Add(new ValidationError(nameof(command.Color), "Color must be a valid hex color code (e.g., #2563EB)"));
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return Task.FromResult(result);
    }

    private bool IsValidHexColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        // Check if it starts with # and has either 3 or 6 hex digits
        if (!color.StartsWith("#"))
            return false;

        var hex = color.Substring(1);
        return (hex.Length == 3 || hex.Length == 6) &&
               hex.All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
    }
}