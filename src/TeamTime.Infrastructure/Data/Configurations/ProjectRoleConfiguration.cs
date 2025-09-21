using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectRoleConfiguration : BaseCatalogConfiguration<ProjectRole>
{
    public override void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        builder.ToTable("project_roles");

        base.Configure(builder);

        // Custom indexes
        builder.HasIndex(pr => pr.Code)
            .IsUnique()
            .HasDatabaseName("ix_project_roles_code");

        // Relationships
        builder.HasMany(pr => pr.ProjectAssignments)
            .WithOne(pa => pa.ProjectRole)
            .HasForeignKey(pa => pa.ProjectRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}