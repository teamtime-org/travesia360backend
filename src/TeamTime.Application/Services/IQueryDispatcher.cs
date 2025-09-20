using TeamTime.Application.Common;

namespace TeamTime.Application.Services;

/// <summary>
/// Service for dispatching queries to their handlers
/// </summary>
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}