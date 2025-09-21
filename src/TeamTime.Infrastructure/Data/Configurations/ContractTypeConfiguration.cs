using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ContractTypeConfiguration : BaseCatalogConfiguration<ContractType>
{
    public override void Configure(EntityTypeBuilder<ContractType> builder)
    {
        builder.ToTable("contract_types");

        base.Configure(builder);

        // Custom indexes
        builder.HasIndex(ct => ct.Code)
            .IsUnique()
            .HasDatabaseName("ix_contract_types_code");

        // Relationships
        builder.HasMany(ct => ct.Projects)
            .WithOne(p => p.ContractType)
            .HasForeignKey(p => p.ContractTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}