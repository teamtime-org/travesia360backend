using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("areas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.HasIndex(a => a.Name)
            .IsUnique();

        builder.Property(a => a.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");

        builder.Property(a => a.Color)
            .IsRequired()
            .HasMaxLength(7)
            .HasDefaultValue("#2563EB")
            .HasColumnName("color");

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(a => a.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(a => a.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(a => a.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Relationships
        builder.HasMany(a => a.Users)
            .WithOne(u => u.Area)
            .HasForeignKey(u => u.AreaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(a => a.Projects)
            .WithOne(p => p.Area)
            .HasForeignKey(p => p.AreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}