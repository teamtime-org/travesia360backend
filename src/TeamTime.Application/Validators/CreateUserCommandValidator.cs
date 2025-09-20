using TeamTime.Application.Features.Users.Commands;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;

namespace TeamTime.Application.Validators;

public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public ValidationResult Validate(CreateUserCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.Email))
        {
            errors.Add(new ValidationError(nameof(command.Email), "Email is required"));
        }
        else if (!IsValidEmail(command.Email))
        {
            errors.Add(new ValidationError(nameof(command.Email), "Email format is invalid"));
        }

        if (string.IsNullOrWhiteSpace(command.FirstName))
        {
            errors.Add(new ValidationError(nameof(command.FirstName), "First name is required"));
        }

        if (string.IsNullOrWhiteSpace(command.LastName))
        {
            errors.Add(new ValidationError(nameof(command.LastName), "Last name is required"));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            errors.Add(new ValidationError(nameof(command.Password), "Password is required"));
        }
        else if (command.Password.Length < 6)
        {
            errors.Add(new ValidationError(nameof(command.Password), "Password must be at least 6 characters"));
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return Task.FromResult(result);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}