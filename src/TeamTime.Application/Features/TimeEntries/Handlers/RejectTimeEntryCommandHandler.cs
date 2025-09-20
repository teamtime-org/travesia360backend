using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class RejectTimeEntryCommandHandler : ICommandHandler<RejectTimeEntryCommand, Result<TimeEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TimeEntry, TimeEntryDto> _mapper;

    public RejectTimeEntryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TimeEntry, TimeEntryDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<TimeEntryDto>> HandleAsync(RejectTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get existing time entry
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(command.Id, cancellationToken);
            if (timeEntry == null)
            {
                return Result<TimeEntryDto>.Failure("Time entry not found");
            }

            if (!timeEntry.IsActive)
            {
                return Result<TimeEntryDto>.Failure("Cannot reject inactive time entry");
            }

            // Reject the time entry
            timeEntry.Reject();

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
            return Result<TimeEntryDto>.Failure($"Error rejecting time entry: {ex.Message}");
        }
    }
}