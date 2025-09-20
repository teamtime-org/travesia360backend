using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
{
    public void Configure(EntityTypeBuilder<JobTitle> builder)
    {
        builder.ToTable("job_titles");

        builder.HasKey(jt => jt.Id);

        builder.Property(jt => jt.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(jt => jt.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.HasIndex(jt => jt.Name)
            .IsUnique();

        builder.Property(jt => jt.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(jt => jt.Department)
            .HasMaxLength(100)
            .HasColumnName("department");

        builder.Property(jt => jt.Level)
            .IsRequired()
            .HasColumnName("level");

        builder.Property(jt => jt.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(jt => jt.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(jt => jt.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(jt => jt.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(jt => jt.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Relationships
        builder.HasMany(jt => jt.Users)
            .WithOne(u => u.JobTitle)
            .HasForeignKey(u => u.JobTitleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}