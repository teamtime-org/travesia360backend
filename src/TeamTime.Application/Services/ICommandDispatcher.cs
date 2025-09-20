using TeamTime.Application.Common;

namespace TeamTime.Application.Services;

/// <summary>
/// Service for dispatching commands to their handlers
/// </summary>
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}