using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Users.Commands;
using TeamTime.Application.Features.Users.Queries;
using TeamTime.Application.Services;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseApiController
{
    public UsersController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Get paginated list of users
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsSuccess && result.Value == null)
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="command">User creation data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetUser),
                new { id = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">User update data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "User not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "User not found")
        {
            return NotFound();
        }

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }
}