using Microsoft.EntityFrameworkCore.Storage;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IAreaRepository? _areas;
    private IProjectRepository? _projects;
    private IRepository<ProjectAssignment>? _projectAssignments;
    private ITaskRepository? _tasks;
    private ITimeEntryRepository? _timeEntries;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public IAreaRepository Areas =>
        _areas ??= new AreaRepository(_context);

    public IProjectRepository Projects =>
        _projects ??= new ProjectRepository(_context);

    public IRepository<ProjectAssignment> ProjectAssignments =>
        _projectAssignments ??= new Repository<ProjectAssignment>(_context);

    public ITaskRepository Tasks =>
        _tasks ??= new TaskRepository(_context);

    public ITimeEntryRepository TimeEntries =>
        _timeEntries ??= new TimeEntryRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}