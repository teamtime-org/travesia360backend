using TeamTime.Domain.Entities;

namespace TeamTime.Domain.Interfaces;

public interface IAreaRepository : IRepository<Area>
{
    Task<Area?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Area>> GetActiveAreasAsync(CancellationToken cancellationToken = default);
    Task<Area?> GetWithUsersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Area?> GetWithProjectsAsync(Guid id, CancellationToken cancellationToken = default);
}