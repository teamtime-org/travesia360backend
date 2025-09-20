using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Users.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Users.Handlers;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<User, UserDto> _mapper;
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper<User, UserDto> mapper,
        IValidator<CreateUserCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<UserDto>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<UserDto>.Failure(errors);
        }

        try
        {
            // Create user entity
            var user = new User(command.Email, command.FirstName, command.LastName, command.Role, command.AreaId);

            // Hash password (simplified - in real implementation use proper password hashing)
            user.UpdatePassword(HashPassword(command.Password));

            // Add to repository
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO and return
            var userDto = _mapper.Map(user);
            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Error creating user: {ex.Message}");
        }
    }

    private string HashPassword(string password)
    {
        // Simplified password hashing - in real implementation use BCrypt or similar
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}