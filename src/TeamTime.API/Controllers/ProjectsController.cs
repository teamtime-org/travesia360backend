using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Projects.Commands;
using TeamTime.Application.Features.Projects.Queries;
using TeamTime.Application.Services;

namespace TeamTime.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ProjectsController : BaseApiController
{
    public ProjectsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
        : base(commandDispatcher, queryDispatcher, loggerFactory)
    {
    }

    /// <summary>
    /// Get paginated list of projects
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of projects</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProjects([FromQuery] GetProjectsQuery query)
    {
        var result = await _queryDispatcher.DispatchAsync(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var query = new GetProjectByIdQuery { Id = id };
        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsSuccess && result.Value == null)
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get projects assigned to a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of user's projects</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(PagedResult<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectsByUser(Guid userId, [FromQuery] GetProjectsByUserQuery query)
    {
        query.UserId = userId;
        var result = await _queryDispatcher.DispatchAsync(query);

        if (result.IsFailure && result.Error == "User not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="command">Project creation data</param>
    /// <returns>Created project</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetProject),
                new { id = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="command">Project update data</param>
    /// <returns>Updated project</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
    {
        command.Id = id;
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Project not found")
        {
            return NotFound();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Delete project (soft delete if has dependencies, hard delete otherwise)
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var command = new DeleteProjectCommand { Id = id };
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure && result.Error == "Project not found")
        {
            return NotFound();
        }

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }

    /// <summary>
    /// Assign user to project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="command">Assignment data</param>
    /// <returns>Success response</returns>
    [HttpPost("{id:guid}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignUserToProject(Guid id, [FromBody] AssignUserToProjectCommand command)
    {
        command.ProjectId = id;
        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure)
        {
            if (result.Error == "Project not found" || result.Error == "User not found")
            {
                return NotFound();
            }
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Remove user from project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="userId">User ID to remove</param>
    /// <param name="removedById">ID of user performing the removal</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}/users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUserFromProject(Guid id, Guid userId, [FromQuery] Guid removedById)
    {
        var command = new RemoveUserFromProjectCommand
        {
            ProjectId = id,
            UserId = userId,
            RemovedById = removedById
        };

        var result = await _commandDispatcher.DispatchAsync(command);

        if (result.IsFailure)
        {
            if (result.Error == "Project not found" || result.Error == "User not found")
            {
                return NotFound();
            }
        }

        return HandleResult(result);
    }
}