using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class BusinessLineConfiguration : BaseCatalogConfiguration<BusinessLine>
{
    public override void Configure(EntityTypeBuilder<BusinessLine> builder)
    {
        builder.ToTable("business_lines");

        base.Configure(builder);

        // Custom indexes
        builder.HasIndex(bl => bl.Code)
            .IsUnique()
            .HasDatabaseName("ix_business_lines_code");

        // Relationships
        builder.HasMany(bl => bl.Projects)
            .WithOne(p => p.BusinessLine)
            .HasForeignKey(p => p.BusinessLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}