using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Projects.Handlers;

public class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Project, ProjectDto> _mapper;
    private readonly IValidator<CreateProjectCommand> _validator;

    public CreateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<Project, ProjectDto> mapper,
        IValidator<CreateProjectCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<ProjectDto>> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
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
            // Verify area exists
            var area = await _unitOfWork.Areas.GetByIdAsync(command.AreaId, cancellationToken);
            if (area == null)
            {
                return Result<ProjectDto>.Failure("Area not found");
            }

            // Create project entity
            var project = new Project(command.Name, command.AreaId, command.Description, command.IsGeneral);

            // Set additional properties
            project.UpdatePriority(command.Priority);

            if (command.StartDate.HasValue || command.EndDate.HasValue)
            {
                project.UpdateDates(command.StartDate, command.EndDate);
            }

            if (command.EstimatedHours.HasValue)
            {
                project.UpdateEstimatedHours(command.EstimatedHours);
            }

            // Set created by
            project.CreatedBy = command.CreatedById;

            // Add to repository
            await _unitOfWork.Projects.AddAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO and return
            var projectDto = _mapper.Map(project);
            return Result<ProjectDto>.Success(projectDto);
        }
        catch (Exception ex)
        {
            return Result<ProjectDto>.Failure($"Error creating project: {ex.Message}");
        }
    }
}