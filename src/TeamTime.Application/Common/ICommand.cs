namespace TeamTime.Application.Common;

/// <summary>
/// Marker interface for commands that return a result
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command</typeparam>
public interface ICommand<TResult>
{
    // Marker interface - no members needed
}