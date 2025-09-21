using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("code");

        builder.Property(c => c.ShortName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("short_name");

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(300)
            .HasColumnName("full_name");

        builder.Property(c => c.SegmentId)
            .IsRequired()
            .HasColumnName("segment_id");

        builder.Property(c => c.IsGovernment)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_government");

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(c => c.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(c => c.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasDatabaseName("ix_clients_code");

        builder.HasIndex(c => c.ShortName)
            .HasDatabaseName("ix_clients_short_name");

        builder.HasIndex(c => c.FullName)
            .HasDatabaseName("ix_clients_full_name");

        builder.HasIndex(c => c.SegmentId)
            .HasDatabaseName("ix_clients_segment_id");

        // Relationships
        builder.HasOne(c => c.Segment)
            .WithMany(s => s.Clients)
            .HasForeignKey(c => c.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Projects)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}