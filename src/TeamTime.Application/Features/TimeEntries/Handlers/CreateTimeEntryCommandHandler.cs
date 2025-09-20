using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class CreateTimeEntryCommandHandler : ICommandHandler<CreateTimeEntryCommand, Result<TimeEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TimeEntry, TimeEntryDto> _mapper;
    private readonly IValidator<CreateTimeEntryCommand> _validator;

    public CreateTimeEntryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<TimeEntry, TimeEntryDto> mapper,
        IValidator<CreateTimeEntryCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<TimeEntryDto>> HandleAsync(CreateTimeEntryCommand command, CancellationToken cancellationToken = default)
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
            // Create time entry entity
            var timeEntry = new TimeEntry(
                command.UserId,
                command.ProjectId,
                command.Date,
                command.Hours,
                command.Description,
                command.TaskId);

            // Add to repository
            await _unitOfWork.TimeEntries.AddAsync(timeEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get the created entity with navigation properties
            var createdEntry = await _unitOfWork.TimeEntries.GetByIdAsync(timeEntry.Id, cancellationToken);

            // Map to DTO and return
            var timeEntryDto = _mapper.Map(createdEntry!);
            return Result<TimeEntryDto>.Success(timeEntryDto);
        }
        catch (Exception ex)
        {
            return Result<TimeEntryDto>.Failure($"Error creating time entry: {ex.Message}");
        }
    }
}