using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class RiskTypeConfiguration : BaseCatalogConfiguration<RiskType>
{
    public override void Configure(EntityTypeBuilder<RiskType> builder)
    {
        builder.ToTable("risk_types");

        base.Configure(builder);

        // Custom indexes
        builder.HasIndex(rt => rt.Code)
            .IsUnique()
            .HasDatabaseName("ix_risk_types_code");

        // Relationships
        builder.HasMany(rt => rt.ProjectRisks)
            .WithOne(pr => pr.RiskType)
            .HasForeignKey(pr => pr.RiskTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}