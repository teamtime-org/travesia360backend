using TeamTime.Application.Features.Auth.Commands;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;

namespace TeamTime.Application.Validators;

public class RefreshTokenCommandValidator : IValidator<RefreshTokenCommand>
{
    public ValidationResult Validate(RefreshTokenCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            errors.Add(new ValidationError(nameof(command.RefreshToken), "Refresh token is required"));
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return Task.FromResult(result);
    }
}