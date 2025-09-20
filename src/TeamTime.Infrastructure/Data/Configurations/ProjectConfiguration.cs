using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.AreaId)
            .IsRequired()
            .HasColumnName("area_id");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.ProjectStatus.ACTIVE);

        builder.Property(p => p.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.Priority.MEDIUM);

        builder.Property(p => p.StartDate)
            .HasColumnName("start_date");

        builder.Property(p => p.EndDate)
            .HasColumnName("end_date");

        builder.Property(p => p.EstimatedHours)
            .HasColumnType("decimal(8,2)")
            .HasColumnName("estimated_hours");

        builder.Property(p => p.IsGeneral)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_general");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(p => p.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(p => p.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Relationships
        builder.HasOne(p => p.Area)
            .WithMany(a => a.Projects)
            .HasForeignKey(p => p.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Assignments)
            .WithOne(pa => pa.Project)
            .HasForeignKey(pa => pa.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.TimeEntries)
            .WithOne(te => te.Project)
            .HasForeignKey(te => te.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}