using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Areas.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Areas.Handlers;

public class CreateAreaCommandHandler : ICommandHandler<CreateAreaCommand, Result<AreaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Area, AreaDto> _mapper;
    private readonly IValidator<CreateAreaCommand> _validator;

    public CreateAreaCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<Area, AreaDto> mapper,
        IValidator<CreateAreaCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<AreaDto>> HandleAsync(CreateAreaCommand command, CancellationToken cancellationToken = default)
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
            // Check if area name already exists
            var existingArea = await _unitOfWork.Areas.GetByNameAsync(command.Name, cancellationToken);
            if (existingArea != null)
            {
                return Result<AreaDto>.Failure("An area with this name already exists");
            }

            // Create area entity
            var area = new Area(command.Name, command.Description, command.Color);

            // Add to repository
            await _unitOfWork.Areas.AddAsync(area, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO and return
            var areaDto = _mapper.Map(area);
            return Result<AreaDto>.Success(areaDto);
        }
        catch (Exception ex)
        {
            return Result<AreaDto>.Failure($"Error creating area: {ex.Message}");
        }
    }
}