using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
{
    public void Configure(EntityTypeBuilder<ProjectAssignment> builder)
    {
        builder.ToTable("project_assignments");

        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(pa => pa.ProjectId)
            .IsRequired()
            .HasColumnName("project_id");

        builder.Property(pa => pa.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(pa => pa.AssignedById)
            .HasColumnName("assigned_by_id");

        builder.Property(pa => pa.AssignedAt)
            .IsRequired()
            .HasColumnName("assigned_at");

        // New fields for import system
        builder.Property(pa => pa.ProjectRoleId)
            .HasColumnName("project_role_id");

        builder.Property(pa => pa.AssignedDate)
            .HasColumnName("assigned_date");

        builder.Property(pa => pa.UnassignedDate)
            .HasColumnName("unassigned_date");

        builder.Property(pa => pa.Notes)
            .HasMaxLength(1000)
            .HasColumnName("notes");

        builder.Property(pa => pa.IsImported)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_imported");

        builder.Property(pa => pa.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(pa => pa.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(pa => pa.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(pa => pa.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(pa => pa.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(pa => new { pa.ProjectId, pa.UserId, pa.IsActive })
            .HasDatabaseName("ix_project_assignments_project_user_active");

        builder.HasIndex(pa => pa.ProjectRoleId)
            .HasDatabaseName("ix_project_assignments_project_role_id");

        builder.HasIndex(pa => pa.IsImported)
            .HasDatabaseName("ix_project_assignments_is_imported");

        builder.HasIndex(pa => pa.AssignedDate)
            .HasDatabaseName("ix_project_assignments_assigned_date");

        // Relationships
        builder.HasOne(pa => pa.User)
            .WithMany(u => u.ProjectAssignments)
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.AssignedBy)
            .WithMany()
            .HasForeignKey(pa => pa.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pa => pa.ProjectRole)
            .WithMany(pr => pr.ProjectAssignments)
            .HasForeignKey(pa => pa.ProjectRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Note: Project relationships are handled conditionally based on IsImported flag
        // Legacy projects use Project, imported projects use ProjectMaster
    }
}