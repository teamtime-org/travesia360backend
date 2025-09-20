using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Domain.Common;

namespace TeamTime.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByAreaIdAsync(Guid areaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<User>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        Guid? areaId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
}