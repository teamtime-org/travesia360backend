using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Common;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        return await _dbSet
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Area)
            .Where(u => u.Role == role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByAreaIdAsync(Guid areaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Area)
            .Where(u => u.AreaId == areaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Area)
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Area)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<User>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        Guid? areaId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(u => u.Area).AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => u.FirstName.Contains(searchTerm) ||
                                   u.LastName.Contains(searchTerm) ||
                                   u.Email.Contains(searchTerm));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (areaId.HasValue)
        {
            query = query.Where(u => u.AreaId == areaId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<User>.Create(items, totalCount, pageNumber, pageSize);
    }
}