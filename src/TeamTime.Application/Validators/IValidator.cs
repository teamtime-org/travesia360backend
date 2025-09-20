namespace TeamTime.Application.Validators;

public interface IValidator<T>
{
    ValidationResult Validate(T instance);
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}