using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;

namespace TeamTime.Infrastructure.Data.Configurations;

public class TimePeriodConfiguration : IEntityTypeConfiguration<TimePeriod>
{
    public void Configure(EntityTypeBuilder<TimePeriod> builder)
    {
        builder.ToTable("time_periods");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(tp => tp.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("name");

        builder.Property(tp => tp.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(tp => tp.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("type");

        builder.Property(tp => tp.StartDate)
            .IsRequired()
            .HasColumnType("date")
            .HasColumnName("start_date");

        builder.Property(tp => tp.EndDate)
            .IsRequired()
            .HasColumnType("date")
            .HasColumnName("end_date");

        builder.Property(tp => tp.ReferenceHours)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasColumnName("reference_hours");

        builder.Property(tp => tp.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(tp => tp.IsCurrent)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_current");

        builder.Property(tp => tp.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(tp => tp.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(tp => tp.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(tp => tp.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(tp => tp.Type)
            .HasDatabaseName("ix_time_periods_type");

        builder.HasIndex(tp => tp.StartDate)
            .HasDatabaseName("ix_time_periods_start_date");

        builder.HasIndex(tp => tp.EndDate)
            .HasDatabaseName("ix_time_periods_end_date");

        builder.HasIndex(tp => tp.IsActive)
            .HasDatabaseName("ix_time_periods_is_active");

        builder.HasIndex(tp => tp.IsCurrent)
            .HasDatabaseName("ix_time_periods_is_current");

        // Unique constraint for current periods to ensure only one current period at a time
        builder.HasIndex(tp => new { tp.IsCurrent, tp.Type })
            .HasDatabaseName("ix_time_periods_current_type")
            .HasFilter("is_current = true");

        // Prevent overlapping periods of the same type
        builder.HasIndex(tp => new { tp.Type, tp.StartDate, tp.EndDate })
            .HasDatabaseName("ix_time_periods_type_dates");

        // Navigation properties configuration
        builder.HasMany(tp => tp.TimeEntries)
            .WithOne(te => te.TimePeriod)
            .HasForeignKey(te => te.TimePeriodId)
            .HasConstraintName("fk_time_entries_time_periods_time_period_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}