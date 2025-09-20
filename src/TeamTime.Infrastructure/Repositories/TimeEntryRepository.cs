using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Common;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Repositories;

public class TimeEntryRepository : Repository<TimeEntry>, ITimeEntryRepository
{
    public TimeEntryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<TimeEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .FirstOrDefaultAsync(te => te.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<TimeEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.UserId == userId && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.ProjectId == projectId && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.Date >= startDate && te.Date <= endDate && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByUserAndDateRangeAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.UserId == userId && te.Date >= startDate && te.Date <= endDate && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByProjectAndDateRangeAsync(Guid projectId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.ProjectId == projectId && te.Date >= startDate && te.Date <= endDate && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => !te.IsApproved && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetApprovedEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.IsApproved && te.IsActive)
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalHoursByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.UserId == userId && te.Date == date && te.IsActive)
            .SumAsync(te => te.Hours, cancellationToken);
    }

    public async Task<decimal> GetTotalHoursByUserAndDateRangeAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.UserId == userId && te.Date >= startDate && te.Date <= endDate && te.IsActive)
            .SumAsync(te => te.Hours, cancellationToken);
    }

    public async Task<bool> UserIsAssignedToProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        // Check if user is assigned to the project through ProjectAssignments
        return await _context.Set<ProjectAssignment>()
            .AnyAsync(pa => pa.UserId == userId && pa.ProjectId == projectId && pa.IsActive, cancellationToken);
    }

    public async Task<PagedResult<TimeEntry>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        Guid? projectId = null,
        Guid? userId = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.IsActive)
            .AsQueryable();

        // Apply filters
        if (startDate.HasValue)
        {
            query = query.Where(te => te.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(te => te.Date <= endDate.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(te => te.ProjectId == projectId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(te => te.UserId == userId.Value);
        }

        if (isApproved.HasValue)
        {
            query = query.Where(te => te.IsApproved == isApproved.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TimeEntry>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<TimeEntry>> GetPagedByUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.UserId == userId && te.IsActive)
            .AsQueryable();

        // Apply filters
        if (startDate.HasValue)
        {
            query = query.Where(te => te.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(te => te.Date <= endDate.Value);
        }

        if (isApproved.HasValue)
        {
            query = query.Where(te => te.IsApproved == isApproved.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TimeEntry>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<TimeEntry>> GetPagedByProjectAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => te.ProjectId == projectId && te.IsActive)
            .AsQueryable();

        // Apply filters
        if (startDate.HasValue)
        {
            query = query.Where(te => te.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(te => te.Date <= endDate.Value);
        }

        if (isApproved.HasValue)
        {
            query = query.Where(te => te.IsApproved == isApproved.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TimeEntry>.Create(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<TimeEntry>> GetPagedPendingApprovalsAsync(
        int pageNumber,
        int pageSize,
        Guid? projectId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            
            .Include(te => te.Project)
            .Include(te => te.Task)
            
            .Where(te => !te.IsApproved && te.IsActive)
            .AsQueryable();

        // Apply filters
        if (projectId.HasValue)
        {
            query = query.Where(te => te.ProjectId == projectId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(te => te.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(te => te.Date <= endDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(te => te.Date)
            .ThenByDescending(te => te.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<TimeEntry>.Create(items, totalCount, pageNumber, pageSize);
    }
}