using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class ApproveTimeEntryCommandHandler : ICommandHandler<ApproveTimeEntryCommand, Result<TimeEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TimeEntry, TimeEntryDto> _mapper;
    private readonly IValidator<ApproveTimeEntryCommand> _validator;

    public ApproveTimeEntryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TimeEntry, TimeEntryDto> mapper,
        IValidator<ApproveTimeEntryCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<TimeEntryDto>> HandleAsync(ApproveTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<TimeEntryDto>.Failure(errors);
        }

        try
        {
            // Get existing time entry
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(command.Id, cancellationToken);
            if (timeEntry == null)
            {
                return Result<TimeEntryDto>.Failure("Time entry not found");
            }

            // Approve the time entry
            timeEntry.Approve(command.ApprovedById);

            // Save changes
            await _unitOfWork.TimeEntries.UpdateAsync(timeEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get updated entity with navigation properties
            var updatedEntry = await _unitOfWork.TimeEntries.GetByIdAsync(timeEntry.Id, cancellationToken);

            // Map to DTO and return
            var timeEntryDto = _mapper.Map(updatedEntry!);
            return Result<TimeEntryDto>.Success(timeEntryDto);
        }
        catch (Exception ex)
        {
            return Result<TimeEntryDto>.Failure($"Error approving time entry: {ex.Message}");
        }
    }
}