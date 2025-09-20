using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Auth.Commands;
using TeamTime.Application.Services;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    public AuthController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <param name="command">Refresh token data</param>
    /// <returns>New authentication response with JWT token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Value);
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
}