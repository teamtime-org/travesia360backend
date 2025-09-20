using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Auth.Commands;
using TeamTime.Application.Services;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IIdentityService _identityService;

    public AuthController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory,
        IIdentityService identityService)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
        _identityService = identityService;
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _identityService.LoginAsync(loginDto.Email, loginDto.Password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token data</param>
    /// <returns>New authentication response with JWT token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _identityService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Logout user (client-side token removal)
    /// </summary>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        // In a JWT stateless system, logout is typically handled client-side
        // by removing the token from storage. Server-side logout would require
        // token blacklisting, which we're not implementing for simplicity.
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Register a new user (Admin only)
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _identityService.RegisterUserAsync(
                registerDto.Email,
                registerDto.Password,
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.Role,
                registerDto.AreaId,
                registerDto.JobTitleId,
                currentUserId);

            return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _identityService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="updateDto">Profile update data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _identityService.UpdateUserProfileAsync(
                userId,
                updateDto.FirstName,
                updateDto.LastName,
                updateDto.PhoneNumber,
                updateDto.EmployeeCode);

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Change current user password
    /// </summary>
    /// <param name="changePasswordDto">Password change data</param>
    /// <returns>Success response</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var success = await _identityService.ChangePasswordAsync(
            userId,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword);

        if (!success)
        {
            return BadRequest(new { message = "Failed to change password. Please check your current password." });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    /// <summary>
    /// Reset user password (Admin only)
    /// </summary>
    /// <param name="resetPasswordDto">Password reset data</param>
    /// <returns>Success response</returns>
    [HttpPost("reset-password")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resetBy = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var success = await _identityService.ResetPasswordAsync(
            resetPasswordDto.UserId,
            resetPasswordDto.NewPassword,
            resetBy);

        if (!success)
        {
            return BadRequest(new { message = "Failed to reset password" });
        }

        return Ok(new { message = "Password reset successfully" });
    }
}