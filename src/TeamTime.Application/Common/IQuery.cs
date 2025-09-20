namespace TeamTime.Application.Common;

/// <summary>
/// Marker interface for queries that return a result
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public interface IQuery<TResult>
{
    // Marker interface - no members needed
}