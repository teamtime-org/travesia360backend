using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Domain.Common;

namespace TeamTime.Domain.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByAreaIdAsync(Guid areaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetOverdueProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetWithAssignmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetWithTimeEntriesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Project>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ProjectStatus? status = null,
        Priority? priority = null,
        Guid? areaId = null,
        bool? isActive = null,
        bool? isGeneral = null,
        bool? isOverdue = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<Project>> GetByUserIdPagedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ProjectStatus? status = null,
        Priority? priority = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
}