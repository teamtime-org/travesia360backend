using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Common;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetByAreaIdAsync(Guid areaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .Where(p => p.AreaId == areaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetOverdueProjectsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _dbSet
            .Include(p => p.Area)
            .Where(p => p.EndDate.HasValue &&
                       p.EndDate < today &&
                       p.Status != ProjectStatus.COMPLETED)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetWithAssignmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .Include(p => p.Assignments)
                .ThenInclude(pa => pa.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetWithTimeEntriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .Include(p => p.TimeEntries)
                .ThenInclude(te => te.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Area)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Project>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ProjectStatus? status = null,
        Priority? priority = null,
        Guid? areaId = null,
        bool? isActive = null,
        bool? isGeneral = null,
        bool? isOverdue = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Area)
            .Include(p => p.Assignments)
            .Include(p => p.Tasks)
            .Include(p => p.TimeEntries)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) ||
                                   (p.Description != null && p.Description.Contains(searchTerm)));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(p => p.Priority == priority.Value);
        }

        if (areaId.HasValue)
        {
            query = query.Where(p => p.AreaId == areaId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        if (isGeneral.HasValue)
        {
            query = query.Where(p => p.IsGeneral == isGeneral.Value);
        }

        if (isOverdue.HasValue && isOverdue.Value)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            query = query.Where(p => p.EndDate.HasValue &&
                               p.EndDate < today &&
                               p.Status != ProjectStatus.COMPLETED);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Project>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Project>> GetByUserIdPagedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ProjectStatus? status = null,
        Priority? priority = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Area)
            .Include(p => p.Assignments)
                .ThenInclude(pa => pa.User)
            .Include(p => p.Tasks)
            .Include(p => p.TimeEntries)
            .Where(p => p.Assignments.Any(pa => pa.UserId == userId && pa.IsActive))
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) ||
                                   (p.Description != null && p.Description.Contains(searchTerm)));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(p => p.Priority == priority.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Project>.Create(items, totalCount, pageNumber, pageSize);
    }
}