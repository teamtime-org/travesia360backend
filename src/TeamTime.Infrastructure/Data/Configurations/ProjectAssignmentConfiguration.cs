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
            .IsRequired()
            .HasColumnName("assigned_by_id");

        builder.Property(pa => pa.AssignedAt)
            .IsRequired()
            .HasColumnName("assigned_at");

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

        // Composite unique index
        builder.HasIndex(pa => new { pa.ProjectId, pa.UserId, pa.IsActive })
            .IsUnique()
            .HasFilter("[is_active] = 1");

        // Relationships
        builder.HasOne(pa => pa.Project)
            .WithMany(p => p.Assignments)
            .HasForeignKey(pa => pa.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.User)
            .WithMany(u => u.ProjectAssignments)
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.AssignedBy)
            .WithMany()
            .HasForeignKey(pa => pa.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}