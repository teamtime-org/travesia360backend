using TeamTime.Application.Common;
using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class DeleteTimeEntryCommandHandler : ICommandHandler<DeleteTimeEntryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTimeEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> HandleAsync(DeleteTimeEntryCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get existing time entry
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(command.Id, cancellationToken);
            if (timeEntry == null)
            {
                return Result.Failure("Time entry not found");
            }

            // Check if entry can be modified (deleted)
            if (!timeEntry.CanBeModified)
            {
                return Result.Failure("Cannot delete approved or inactive time entry");
            }

            // Soft delete by deactivating
            timeEntry.Deactivate();

            // Save changes
            await _unitOfWork.TimeEntries.UpdateAsync(timeEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting time entry: {ex.Message}");
        }
    }
}