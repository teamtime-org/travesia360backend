using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Repositories;

public class AreaRepository : Repository<Area>, IAreaRepository
{
    public AreaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Area?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        return await _dbSet
            .FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        return await _dbSet.AnyAsync(a => a.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Area>> GetActiveAreasAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Area?> GetWithUsersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Users)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Area?> GetWithProjectsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Projects)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}