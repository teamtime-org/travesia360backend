namespace TeamTime.Application.Validators;

public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }

    public ValidationError()
    {
    }

    public ValidationError(string propertyName, string errorMessage, object? attemptedValue = null)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        AttemptedValue = attemptedValue;
    }
}