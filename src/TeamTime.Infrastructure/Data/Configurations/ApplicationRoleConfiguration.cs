using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Infrastructure.Identity;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // Configure additional properties
        builder.Property(r => r.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        // Configure inherited Identity properties
        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.Name)
            .HasColumnName("name");

        builder.Property(r => r.NormalizedName)
            .HasColumnName("normalized_name");

        builder.Property(r => r.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");
    }
}