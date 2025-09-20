using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("time_entries");

        builder.HasKey(te => te.Id);

        builder.Property(te => te.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(te => te.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(te => te.ProjectId)
            .IsRequired()
            .HasColumnName("project_id");

        builder.Property(te => te.TaskId)
            .HasColumnName("task_id");

        builder.Property(te => te.Date)
            .IsRequired()
            .HasColumnName("date");

        builder.Property(te => te.Hours)
            .IsRequired()
            .HasColumnType("decimal(4,2)")
            .HasColumnName("hours");

        builder.Property(te => te.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");

        builder.Property(te => te.IsApproved)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_approved");

        builder.Property(te => te.ApprovedById)
            .HasColumnName("approved_by_id");

        builder.Property(te => te.ApprovedAt)
            .HasColumnName("approved_at");

        builder.Property(te => te.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(te => te.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(te => te.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(te => te.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(te => te.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(te => new { te.UserId, te.Date })
            .HasDatabaseName("IX_time_entries_user_date");

        builder.HasIndex(te => new { te.ProjectId, te.Date })
            .HasDatabaseName("IX_time_entries_project_date");

        // Relationships
        builder.HasOne(te => te.User)
            .WithMany(u => u.TimeEntries)
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(te => te.Project)
            .WithMany(p => p.TimeEntries)
            .HasForeignKey(te => te.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(te => te.Task)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(te => te.ApprovedBy)
            .WithMany(u => u.ApprovedTimeEntries)
            .HasForeignKey(te => te.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}