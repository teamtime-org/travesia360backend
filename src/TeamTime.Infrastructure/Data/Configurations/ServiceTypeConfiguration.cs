using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ServiceTypeConfiguration : BaseCatalogConfiguration<ServiceType>
{
    public override void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.ToTable("service_types");

        base.Configure(builder);

        // Custom indexes
        builder.HasIndex(st => st.Code)
            .IsUnique()
            .HasDatabaseName("ix_service_types_code");

        // Relationships
        builder.HasMany(st => st.Projects)
            .WithOne(p => p.ServiceType)
            .HasForeignKey(p => p.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}