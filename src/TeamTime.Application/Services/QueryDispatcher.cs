using TeamTime.Application.Common;

namespace TeamTime.Application.Services;

/// <summary>
/// Implementation of query dispatcher using native dependency injection
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler found for query {queryType.Name}");

        // Use dynamic to call the HandleAsync method
        dynamic handlerDynamic = handler;
        return await handlerDynamic.HandleAsync((dynamic)query, cancellationToken);
    }
}