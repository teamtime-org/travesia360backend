using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Tasks.Commands;
using TeamTime.Application.Features.Tasks.Queries;
using TeamTime.Application.Services;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TasksController : BaseApiController
{
    public TasksController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Get paginated list of tasks
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of tasks</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var query = new GetTaskByIdQuery { Id = id };
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get tasks by project ID
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of tasks for the project</returns>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTasksByProject(Guid projectId, [FromQuery] GetTasksByProjectQuery query)
    {
        query.ProjectId = projectId;
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get overdue tasks
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of overdue tasks</returns>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOverdueTasks([FromQuery] GetOverdueTasksQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="dto">Task creation data</param>
    /// <returns>Created task</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        var command = new CreateTaskCommand
        {
            Name = dto.Name,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            EstimatedHours = dto.EstimatedHours
        };

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetTask), new { id = result.Value!.Id }, result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="dto">Task update data</param>
    /// <returns>Updated task</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var command = new UpdateTaskCommand
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            EstimatedHours = dto.EstimatedHours
        };

        var result = await _commandDispatcher.DispatchAsync(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a task (soft delete)
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Success or error</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var command = new DeleteTaskCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Change task status
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="dto">Status change data</param>
    /// <returns>Updated task</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeTaskStatus(Guid id, [FromBody] ChangeTaskStatusDto dto)
    {
        var command = new ChangeTaskStatusCommand
        {
            Id = id,
            Status = dto.Status
        };

        var result = await _commandDispatcher.DispatchAsync(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Start a task (change status to IN_PROGRESS)
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Updated task</returns>
    [HttpPatch("{id:guid}/start")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartTask(Guid id)
    {
        var command = new StartTaskCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Complete a task (change status to DONE)
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Updated task</returns>
    [HttpPatch("{id:guid}/complete")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteTask(Guid id)
    {
        var command = new CompleteTaskCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);
        return HandleResult(result);
    }
}