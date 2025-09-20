using TeamTime.Application.Common;
using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Unit>> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get existing project
            var project = await _unitOfWork.Projects.GetByIdAsync(command.Id, cancellationToken);
            if (project == null)
            {
                return Result<Unit>.Failure("Project not found");
            }

            // Check if project has dependencies (assignments, tasks, time entries)
            var projectWithDependencies = await _unitOfWork.Projects.GetWithAssignmentsAsync(command.Id, cancellationToken);
            if (projectWithDependencies?.Assignments.Any() == true ||
                projectWithDependencies?.Tasks.Any() == true ||
                projectWithDependencies?.TimeEntries.Any() == true)
            {
                // Instead of hard delete, deactivate the project
                project.Deactivate();
                project.LastModifiedBy = command.DeletedById;
                await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);
            }
            else
            {
                // Hard delete if no dependencies
                await _unitOfWork.Projects.DeleteAsync(project, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit>.Failure($"Error deleting project: {ex.Message}");
        }
    }
}