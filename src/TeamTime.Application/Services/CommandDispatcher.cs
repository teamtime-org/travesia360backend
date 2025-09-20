using TeamTime.Application.Common;

namespace TeamTime.Application.Services;

/// <summary>
/// Implementation of command dispatcher using native dependency injection
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler found for command {commandType.Name}");

        // Use dynamic to call the HandleAsync method
        dynamic handlerDynamic = handler;
        return await handlerDynamic.HandleAsync((dynamic)command, cancellationToken);
    }
}