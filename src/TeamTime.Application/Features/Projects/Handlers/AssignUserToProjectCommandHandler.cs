using TeamTime.Application.Common;
using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class AssignUserToProjectCommandHandler : ICommandHandler<AssignUserToProjectCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AssignUserToProjectCommand> _validator;

    public AssignUserToProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<AssignUserToProjectCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<Unit>> HandleAsync(AssignUserToProjectCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<Unit>.Failure(errors);
        }

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

            // Verify assigner exists
            var assigner = await _unitOfWork.Users.GetByIdAsync(command.AssignedById, cancellationToken);
            if (assigner == null)
            {
                return Result<Unit>.Failure("Assigner not found");
            }

            // Check if user is already assigned to the project
            // Check if user is already assigned to the project
            var existingAssignment = project.Assignments.FirstOrDefault(a => a.UserId == command.UserId);
            if (existingAssignment != null)
            {
                if (existingAssignment.IsActive)
                {
                    return Result<Unit>.Failure("User is already assigned to this project");
                }
                else
                {
                    // Reactivate existing assignment
                    existingAssignment.Reactivate();
                }
            }
            else
            {
                // Create new assignment
                var assignment = new ProjectAssignment(command.ProjectId, command.UserId, command.AssignedById);
                await _unitOfWork.ProjectAssignments.AddAsync(assignment, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit>.Failure($"Error assigning user to project: {ex.Message}");
        }
    }
}