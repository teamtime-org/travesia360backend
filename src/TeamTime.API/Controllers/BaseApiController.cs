using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.Services;
using TeamTime.Common.Results;
using TeamTime.API.Models;

namespace TeamTime.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ICommandDispatcher _commandDispatcher;
    protected readonly IQueryDispatcher _queryDispatcher;
    protected readonly ILogger _logger;

    protected BaseApiController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILoggerFactory loggerFactory)
    {
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        _logger = loggerFactory?.CreateLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                Data = result.Value
            });
        }

        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Message = result.Error,
            Errors = result.Errors
        });
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = result.Error
        });
    }
}