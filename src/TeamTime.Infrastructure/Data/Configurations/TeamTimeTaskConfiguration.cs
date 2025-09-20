using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class TeamTimeTaskConfiguration : IEntityTypeConfiguration<TeamTimeTask>
{
    public void Configure(EntityTypeBuilder<TeamTimeTask> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.ProjectId)
            .IsRequired()
            .HasColumnName("project_id");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.TaskStatus.TODO);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.Priority.MEDIUM);

        builder.Property(t => t.DueDate)
            .HasColumnName("due_date");

        builder.Property(t => t.EstimatedHours)
            .HasColumnType("decimal(8,2)")
            .HasColumnName("estimated_hours");

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(t => t.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Relationships
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TimeEntries)
            .WithOne(te => te.Task)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}