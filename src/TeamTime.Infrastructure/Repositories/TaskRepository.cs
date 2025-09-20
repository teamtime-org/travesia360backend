using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Common;
using TeamTime.Infrastructure.Data;
using TaskStatus = TeamTime.Domain.Enums.TaskStatus;
using TeamTime.Domain.Enums;

namespace TeamTime.Infrastructure.Repositories;

public class TaskRepository : Repository<TeamTimeTask>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TeamTimeTask>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamTimeTask>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamTimeTask>> GetByPriorityAsync(Priority priority, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.Priority == priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamTimeTask>> GetActiveTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamTimeTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.DueDate.HasValue &&
                       t.DueDate < today &&
                       t.Status != TaskStatus.DONE)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamTimeTask>> GetCompletedTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.Status == TaskStatus.DONE)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamTimeTask>> GetInProgressTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.Status == TaskStatus.IN_PROGRESS)
            .ToListAsync(cancellationToken);
    }

    public async Task<TeamTimeTask?> GetWithProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<TeamTimeTask?> GetWithTimeEntriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.TimeEntries)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<PagedResult<TeamTimeTask>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TaskStatus? status = null,
        Priority? priority = null,
        Guid? projectId = null,
        bool? isActive = null,
        bool? isOverdue = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchTerm) ||
                                    (t.Description != null && t.Description.Contains(searchTerm)));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(t => t.IsActive == isActive.Value);
        }

        if (isOverdue.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (isOverdue.Value)
            {
                query = query.Where(t => t.DueDate.HasValue &&
                                        t.DueDate < today &&
                                        t.Status != TaskStatus.DONE);
            }
            else
            {
                query = query.Where(t => !t.DueDate.HasValue ||
                                        t.DueDate >= today ||
                                        t.Status == TaskStatus.DONE);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TeamTimeTask>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<TeamTimeTask>> GetByProjectIdPagedAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TaskStatus? status = null,
        Priority? priority = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            pageNumber,
            pageSize,
            searchTerm,
            status,
            priority,
            projectId,
            isActive,
            null,
            cancellationToken);
    }

    public async Task<PagedResult<TeamTimeTask>> GetOverdueTasksPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            pageNumber,
            pageSize,
            searchTerm,
            null,
            null,
            projectId,
            null,
            true,
            cancellationToken);
    }
}