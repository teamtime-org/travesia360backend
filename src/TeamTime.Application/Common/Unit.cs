namespace TeamTime.Application.Common;

/// <summary>
/// Unit type for commands that don't return a value
/// </summary>
public struct Unit
{
    public static readonly Unit Value = new();
}