using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Common;
using TeamTime.Domain.Entities;
using TeamTime.Infrastructure.Data.Extensions;
using TeamTime.Infrastructure.Identity;

namespace TeamTime.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Domain entities
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectAssignment> ProjectAssignments => Set<ProjectAssignment>();
    public DbSet<TeamTimeTask> Tasks => Set<TeamTimeTask>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<TimePeriod> TimePeriods => Set<TimePeriod>();
    public DbSet<JobTitle> JobTitles => Set<JobTitle>();

    // Import System Entities
    // Catalogs
    public DbSet<Segment> Segments => Set<Segment>();
    public DbSet<ProjectStage> ProjectStages => Set<ProjectStage>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<ContractType> ContractTypes => Set<ContractType>();
    public DbSet<BusinessLine> BusinessLines => Set<BusinessLine>();
    public DbSet<RiskLevel> RiskLevels => Set<RiskLevel>();
    public DbSet<RiskType> RiskTypes => Set<RiskType>();
    public DbSet<ProjectRole> ProjectRoles => Set<ProjectRole>();

    // Main Entities
    public DbSet<Client> Clients => Set<Client>();

    // Relational Entities
    public DbSet<ProjectRisk> ProjectRisks => Set<ProjectRisk>();
    public DbSet<ProjectStageHistory> ProjectStageHistory => Set<ProjectStageHistory>();

    // Identity entities are inherited from IdentityDbContext

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Identity table names
        modelBuilder.Entity<ApplicationUser>().ToTable("users");
        modelBuilder.Entity<ApplicationRole>().ToTable("roles");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("user_roles");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("user_claims");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("user_logins");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("user_tokens");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("role_claims");

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply snake_case naming convention to all entities
        modelBuilder.ApplySnakeCaseNamingConvention();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps for entities
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events (if needed in the future)
        await DispatchEventsAsync();

        return result;
    }

    private async Task DispatchEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        // Here you would dispatch the events to handlers
        // For now, we'll just clear them
        await Task.CompletedTask;
    }
}