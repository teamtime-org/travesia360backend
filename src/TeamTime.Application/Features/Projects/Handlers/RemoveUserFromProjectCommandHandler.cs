using TeamTime.Application.Common;
using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class RemoveUserFromProjectCommandHandler : ICommandHandler<RemoveUserFromProjectCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserFromProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Unit>> HandleAsync(RemoveUserFromProjectCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify project exists
            var project = await _unitOfWork.Projects.GetWithAssignmentsAsync(command.ProjectId, cancellationToken);
            if (project == null)
            {
                return Result<Unit>.Failure("Project not found");
            }

            // Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                return Result<Unit>.Failure("User not found");
            }

            // Verify remover exists
            var remover = await _unitOfWork.Users.GetByIdAsync(command.RemovedById, cancellationToken);
            if (remover == null)
            {
                return Result<Unit>.Failure("Remover not found");
            }

            // Find active assignment
            // Find active assignment
            var assignment = project.Assignments.FirstOrDefault(a => a.UserId == command.UserId && a.IsActive);
            if (assignment == null)
            {
                return Result<Unit>.Failure("User is not assigned to this project");
            }

            // Deactivate assignment instead of hard delete to maintain history
            // Deactivate assignment instead of hard delete to maintain history
            assignment.Deactivate();
            assignment.LastModifiedBy = command.RemovedById;

            await _unitOfWork.ProjectAssignments.UpdateAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit>.Failure($"Error removing user from project: {ex.Message}");
        }
    }
}