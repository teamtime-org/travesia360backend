using TeamTime.Application.Features.Auth.Commands;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;

namespace TeamTime.Application.Validators;

public class LoginCommandValidator : IValidator<LoginCommand>
{
    public ValidationResult Validate(LoginCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(LoginCommand command, CancellationToken cancellationToken = default)
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

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            errors.Add(new ValidationError(nameof(command.Password), "Password is required"));
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