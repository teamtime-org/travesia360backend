using TeamTime.Application.Common;
using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Tasks.Handlers;

public class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> HandleAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find task
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.Id, cancellationToken);
            if (task == null)
            {
                return Result.Failure("Task not found");
            }

            // Soft delete by deactivating
            task.Deactivate();

            // Save changes
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete task: {ex.Message}");
        }
    }
}