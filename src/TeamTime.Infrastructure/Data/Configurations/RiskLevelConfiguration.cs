using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class RiskLevelConfiguration : BaseCatalogConfiguration<RiskLevel>
{
    public override void Configure(EntityTypeBuilder<RiskLevel> builder)
    {
        builder.ToTable("risk_levels");

        base.Configure(builder);

        // Additional properties specific to RiskLevel
        builder.Property(rl => rl.Color)
            .HasMaxLength(20)
            .HasColumnName("color");

        // Custom indexes
        builder.HasIndex(rl => rl.Code)
            .IsUnique()
            .HasDatabaseName("ix_risk_levels_code");

        // Relationships
        builder.HasMany(rl => rl.ProjectRisks)
            .WithOne(pr => pr.RiskLevel)
            .HasForeignKey(pr => pr.RiskLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}