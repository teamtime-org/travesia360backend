using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Domain.Common;

namespace TeamTime.Domain.Interfaces;

public interface ITaskRepository : IRepository<TeamTimeTask>
{
    Task<IEnumerable<TeamTimeTask>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamTimeTask>> GetByStatusAsync(Enums.TaskStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamTimeTask>> GetByPriorityAsync(Priority priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamTimeTask>> GetActiveTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamTimeTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamTimeTask>> GetCompletedTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamTimeTask>> GetInProgressTasksAsync(CancellationToken cancellationToken = default);
    Task<TeamTimeTask?> GetWithProjectAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TeamTimeTask?> GetWithTimeEntriesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TeamTimeTask>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Enums.TaskStatus? status = null,
        Priority? priority = null,
        Guid? projectId = null,
        bool? isActive = null,
        bool? isOverdue = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<TeamTimeTask>> GetByProjectIdPagedAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Enums.TaskStatus? status = null,
        Priority? priority = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<TeamTimeTask>> GetOverdueTasksPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);
}