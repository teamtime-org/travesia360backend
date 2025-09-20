using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Project, ProjectDto> _mapper;
    private readonly IValidator<UpdateProjectCommand> _validator;

    public UpdateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<Project, ProjectDto> mapper,
        IValidator<UpdateProjectCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<ProjectDto>> HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<ProjectDto>.Failure(errors);
        }

        try
        {
            // Get existing project
            var project = await _unitOfWork.Projects.GetByIdAsync(command.Id, cancellationToken);
            if (project == null)
            {
                return Result<ProjectDto>.Failure("Project not found");
            }

            // Update project properties
            project.UpdateDetails(command.Name, command.Description);
            project.UpdateStatus(command.Status);
            project.UpdatePriority(command.Priority);

            if (command.StartDate.HasValue || command.EndDate.HasValue)
            {
                project.UpdateDates(command.StartDate, command.EndDate);
            }

            if (command.EstimatedHours.HasValue)
            {
                project.UpdateEstimatedHours(command.EstimatedHours);
            }

            // Set last modified by
            project.LastModifiedBy = command.UpdatedById;

            // Update in repository
            await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO and return
            var projectDto = _mapper.Map(project);
            return Result<ProjectDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            return Result<ProjectDto>.Failure($"Error updating project: {ex.Message}");
        }
    }
}