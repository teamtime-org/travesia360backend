using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Areas.Commands;
using TeamTime.Application.Features.Areas.Queries;
using TeamTime.Application.Services;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class AreasController : BaseApiController
{
    public AreasController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Get paginated list of areas
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of areas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AreaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAreas([FromQuery] GetAreasQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get area by ID
    /// </summary>
    /// <param name="id">Area ID</param>
    /// <returns>Area details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AreaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetArea(Guid id)
    {
        var query = new GetAreaByIdQuery { Id = id };
        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsSuccess && result.Value == null)
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Create a new area
    /// </summary>
    /// <param name="command">Area creation data</param>
    /// <returns>Created area</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AreaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateArea([FromBody] CreateAreaCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetArea),
                new { id = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update area
    /// </summary>
    /// <param name="id">Area ID</param>
    /// <param name="command">Area update data</param>
    /// <returns>Updated area</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AreaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateArea(Guid id, [FromBody] UpdateAreaCommand command)
    {
        command.Id = id;
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Area not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Delete area (soft delete if has associations, hard delete otherwise)
    /// </summary>
    /// <param name="id">Area ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteArea(Guid id)
    {
        var command = new DeleteAreaCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Area not found")
        {
            return NotFound();
        }

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }
}