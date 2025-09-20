using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Areas.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Areas.Handlers;

public class UpdateAreaCommandHandler : ICommandHandler<UpdateAreaCommand, Result<AreaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Area, AreaDto> _mapper;
    private readonly IValidator<UpdateAreaCommand> _validator;

    public UpdateAreaCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<Area, AreaDto> mapper,
        IValidator<UpdateAreaCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<AreaDto>> HandleAsync(UpdateAreaCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<AreaDto>.Failure(errors);
        }

        try
        {
            // Get existing area
            var area = await _unitOfWork.Areas.GetByIdAsync(command.Id, cancellationToken);
            if (area == null)
            {
                return Result<AreaDto>.Failure("Area not found");
            }

            // Check if name already exists (but not for the same area)
            var existingAreaWithName = await _unitOfWork.Areas.GetByNameAsync(command.Name, cancellationToken);
            if (existingAreaWithName != null && existingAreaWithName.Id != command.Id)
            {
                return Result<AreaDto>.Failure("An area with this name already exists");
            }

            // Update area
            area.UpdateDetails(command.Name, command.Description, command.Color);

            if (command.IsActive.HasValue)
            {
                if (command.IsActive.Value)
                    area.Activate();
                else
                    area.Deactivate();
            }

            await _unitOfWork.Areas.UpdateAsync(area, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO and return
            var areaDto = _mapper.Map(area);
            return Result<AreaDto>.Success(areaDto);
        }
        catch (Exception ex)
        {
            return Result<AreaDto>.Failure($"Error updating area: {ex.Message}");
        }
    }
}