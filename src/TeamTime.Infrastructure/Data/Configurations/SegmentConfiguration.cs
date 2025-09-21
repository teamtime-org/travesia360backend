using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class SegmentConfiguration : BaseCatalogConfiguration<Segment>
{
    public override void Configure(EntityTypeBuilder<Segment> builder)
    {
        builder.ToTable("segments");

        base.Configure(builder);

        // Custom indexes with specific names
        builder.HasIndex(s => s.Code)
            .IsUnique()
            .HasDatabaseName("ix_segments_code");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("ix_segments_name");

        builder.HasIndex(s => s.DisplayOrder)
            .HasDatabaseName("ix_segments_display_order");

        // Relationships
        builder.HasMany(s => s.Clients)
            .WithOne(c => c.Segment)
            .HasForeignKey(c => c.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Projects)
            .WithOne(p => p.Segment)
            .HasForeignKey(p => p.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}