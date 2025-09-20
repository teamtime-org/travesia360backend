namespace TeamTime.Application.Common;

/// <summary>
/// Marker interface for commands that don't return a result
/// </summary>
public interface ICommand : ICommand<Unit>
{
    // Marker interface - no members needed
}