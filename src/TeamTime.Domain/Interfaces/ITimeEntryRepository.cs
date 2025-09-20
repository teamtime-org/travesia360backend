using TeamTime.Domain.Entities;
using TeamTime.Domain.Common;

namespace TeamTime.Domain.Interfaces;

public interface ITimeEntryRepository : IRepository<TimeEntry>
{
    Task<IEnumerable<TimeEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByUserAndDateRangeAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByProjectAndDateRangeAsync(Guid projectId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetApprovedEntriesAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursByUserAndDateRangeAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<bool> UserIsAssignedToProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    Task<PagedResult<TimeEntry>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        Guid? projectId = null,
        Guid? userId = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<TimeEntry>> GetPagedByUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<TimeEntry>> GetPagedByProjectAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<TimeEntry>> GetPagedPendingApprovalsAsync(
        int pageNumber,
        int pageSize,
        Guid? projectId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);
}