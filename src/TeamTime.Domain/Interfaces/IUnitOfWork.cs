namespace TeamTime.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IAreaRepository Areas { get; }
    IProjectRepository Projects { get; }
    IRepository<TeamTime.Domain.Entities.ProjectAssignment> ProjectAssignments { get; }
    ITaskRepository Tasks { get; }
    ITimeEntryRepository TimeEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}