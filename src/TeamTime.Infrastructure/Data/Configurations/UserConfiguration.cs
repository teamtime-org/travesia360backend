using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;

namespace TeamTime.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("password_hash");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("first_name");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("last_name");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnName("role");

        builder.Property(u => u.AreaId)
            .HasColumnName("area_id");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(u => u.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(u => u.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Relationships
        builder.HasOne(u => u.Area)
            .WithMany(a => a.Users)
            .HasForeignKey(u => u.AreaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.ProjectAssignments)
            .WithOne(pa => pa.User)
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.CreatedProjects)
            .WithOne()
            .HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.CreatedTasks)
            .WithOne()
            .HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.TimeEntries)
            .WithOne(te => te.User)
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ApprovedTimeEntries)
            .WithOne(te => te.ApprovedBy)
            .HasForeignKey(te => te.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}