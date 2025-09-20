using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Validators;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Validators;

public class UpdateTimeEntryCommandValidator : IValidator<UpdateTimeEntryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTimeEntryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public ValidationResult Validate(UpdateTimeEntryCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public async Task<ValidationResult> ValidateAsync(UpdateTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.Id), "Time entry ID is required"));
        }

        if (command.Hours < 0.25m)
        {
            errors.Add(new ValidationError(nameof(command.Hours), "Hours must be at least 0.25 (15 minutes)"));
        }

        if (command.Hours > 24)
        {
            errors.Add(new ValidationError(nameof(command.Hours), "Hours cannot exceed 24 per day"));
        }

        // Check if time entry exists
        if (command.Id != Guid.Empty)
        {
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(command.Id, cancellationToken);
            if (timeEntry == null)
            {
                errors.Add(new ValidationError(nameof(command.Id), "Time entry not found"));
            }
            else
            {
                // Check if entry can be modified
                if (!timeEntry.CanBeModified)
                {
                    errors.Add(new ValidationError(nameof(command.Id), "Cannot modify approved or inactive time entry"));
                }

                // Check daily hours limit (excluding current entry)
                var currentDayHours = await _unitOfWork.TimeEntries.GetTotalHoursByUserAndDateAsync(timeEntry.UserId, timeEntry.Date, cancellationToken);
                var hoursWithoutCurrent = currentDayHours - timeEntry.Hours;
                if (hoursWithoutCurrent + command.Hours > 24)
                {
                    errors.Add(new ValidationError(nameof(command.Hours), $"Total daily hours would exceed 24. Current (excluding this entry): {hoursWithoutCurrent}, Attempting to set: {command.Hours}"));
                }
            }
        }

        // Check if task exists (if provided)
        if (command.TaskId.HasValue)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.TaskId.Value, cancellationToken);
            if (task == null)
            {
                errors.Add(new ValidationError(nameof(command.TaskId), "Task not found"));
            }
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };

        return result;
    }
}