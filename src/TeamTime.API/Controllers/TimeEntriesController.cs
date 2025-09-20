using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Commands;
using TeamTime.Application.Features.TimeEntries.Queries;
using TeamTime.Application.Services;
using TeamTime.Domain.Common;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TimeEntriesController : BaseApiController
{
    public TimeEntriesController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Get paginated list of time entries
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of time entries</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTimeEntries([FromQuery] GetTimeEntriesQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get time entry by ID
    /// </summary>
    /// <param name="id">Time entry ID</param>
    /// <returns>Time entry details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTimeEntry(Guid id)
    {
        var query = new GetTimeEntryByIdQuery { Id = id };
        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsFailure && result.Error == "Time entry not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get time entries by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Time entries for the specified user</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTimeEntriesByUser(Guid userId, [FromQuery] GetTimeEntriesByUserQuery query)
    {
        query.UserId = userId;
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get time entries by project ID
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Time entries for the specified project</returns>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTimeEntriesByProject(Guid projectId, [FromQuery] GetTimeEntriesByProjectQuery query)
    {
        query.ProjectId = projectId;
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get pending time entries awaiting approval
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Time entries pending approval</returns>
    [HttpGet("pending-approvals")]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPendingApprovals([FromQuery] GetPendingApprovalsQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new time entry
    /// </summary>
    /// <param name="command">Time entry creation data</param>
    /// <returns>Created time entry</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTimeEntry([FromBody] CreateTimeEntryCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetTimeEntry),
                new { id = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update time entry
    /// </summary>
    /// <param name="id">Time entry ID</param>
    /// <param name="command">Time entry update data</param>
    /// <returns>Updated time entry</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTimeEntry(Guid id, [FromBody] UpdateTimeEntryCommand command)
    {
        command.Id = id;
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Time entry not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Delete time entry (soft delete)
    /// </summary>
    /// <param name="id">Time entry ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTimeEntry(Guid id)
    {
        var command = new DeleteTimeEntryCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Time entry not found")
        {
            return NotFound();
        }

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }

    /// <summary>
    /// Approve time entry
    /// </summary>
    /// <param name="id">Time entry ID</param>
    /// <param name="command">Approval data</param>
    /// <returns>Approved time entry</returns>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ApproveTimeEntry(Guid id, [FromBody] ApproveTimeEntryCommand command)
    {
        command.Id = id;
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Time entry not found")
        {
            return NotFound();
        }

        if (result.IsFailure && result.Error.Contains("permission"))
        {
            return Forbid();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Reject time entry
    /// </summary>
    /// <param name="id">Time entry ID</param>
    /// <returns>Rejected time entry</returns>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectTimeEntry(Guid id)
    {
        var command = new RejectTimeEntryCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Time entry not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }
}