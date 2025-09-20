using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Validators;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Validators;

public class ApproveTimeEntryCommandValidator : IValidator<ApproveTimeEntryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveTimeEntryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public ValidationResult Validate(ApproveTimeEntryCommand command)
    {
        return ValidateAsync(command).GetAwaiter().GetResult();
    }

    public async Task<ValidationResult> ValidateAsync(ApproveTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (command.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.Id), "Time entry ID is required"));
        }

        if (command.ApprovedById == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(command.ApprovedById), "Approver ID is required"));
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
                if (timeEntry.IsApproved)
                {
                    errors.Add(new ValidationError(nameof(command.Id), "Time entry is already approved"));
                }

                if (!timeEntry.IsActive)
                {
                    errors.Add(new ValidationError(nameof(command.Id), "Cannot approve inactive time entry"));
                }
            }
        }

        // Check if approver exists and has permission
        if (command.ApprovedById != Guid.Empty)
        {
            var approver = await _unitOfWork.Users.GetByIdAsync(command.ApprovedById, cancellationToken);
            if (approver == null)
            {
                errors.Add(new ValidationError(nameof(command.ApprovedById), "Approver not found"));
            }
            else
            {
                // Only coordinators and administrators can approve
                if (approver.Role != UserRole.COORDINADOR && approver.Role != UserRole.ADMINISTRADOR)
                {
                    errors.Add(new ValidationError(nameof(command.ApprovedById), "User does not have permission to approve time entries"));
                }
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