using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Auth.Commands;
using TeamTime.Application.Mappings;
using TeamTime.Application.Services;
using TeamTime.Application.Validators;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Auth.Handlers;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper<User, UserDto> _userMapper;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMapper<User, UserDto> userMapper,
        IValidator<LoginCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<AuthResponseDto>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<AuthResponseDto>.Failure(errors);
        }

        try
        {
            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(command.Email, cancellationToken);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("User account is deactivated");
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var expiresAt = _tokenService.GetTokenExpiration(accessToken);

            // Map user to DTO
            var userDto = _userMapper.Map(user);

            var authResponse = new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = userDto
            };

            return Result<AuthResponseDto>.Success(authResponse);
        }
        catch (Exception ex)
        {
            return Result<AuthResponseDto>.Failure($"Login failed: {ex.Message}");
        }
    }
}