namespace TeamTime.Application.Common;

/// <summary>
/// Handler for commands that don't return a result
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand
{
    // Inherits from generic handler
}