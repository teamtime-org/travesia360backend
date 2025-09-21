using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectStageConfiguration : BaseCatalogConfiguration<ProjectStage>
{
    public override void Configure(EntityTypeBuilder<ProjectStage> builder)
    {
        builder.ToTable("project_stages");

        base.Configure(builder);

        // Additional properties specific to ProjectStage
        builder.Property(ps => ps.Phase)
            .HasMaxLength(50)
            .HasColumnName("phase");

        builder.Property(ps => ps.Color)
            .HasMaxLength(20)
            .HasColumnName("color");

        // Custom indexes
        builder.HasIndex(ps => ps.Code)
            .IsUnique()
            .HasDatabaseName("ix_project_stages_code");

        builder.HasIndex(ps => ps.Phase)
            .HasDatabaseName("ix_project_stages_phase");

        // Relationships
        builder.HasMany(ps => ps.Projects)
            .WithOne(p => p.ProjectStage)
            .HasForeignKey(p => p.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ps => ps.StageHistories)
            .WithOne(psh => psh.ProjectStage)
            .HasForeignKey(psh => psh.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}