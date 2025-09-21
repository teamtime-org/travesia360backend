using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectRiskConfiguration : IEntityTypeConfiguration<ProjectRisk>
{
    public void Configure(EntityTypeBuilder<ProjectRisk> builder)
    {
        builder.ToTable("project_risks");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(pr => pr.ProjectId)
            .IsRequired()
            .HasColumnName("project_id");

        builder.Property(pr => pr.RiskTypeId)
            .IsRequired()
            .HasColumnName("risk_type_id");

        builder.Property(pr => pr.RiskLevelId)
            .IsRequired()
            .HasColumnName("risk_level_id");

        builder.Property(pr => pr.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("title");

        builder.Property(pr => pr.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");

        builder.Property(pr => pr.Mitigation)
            .HasMaxLength(1000)
            .HasColumnName("mitigation");

        builder.Property(pr => pr.IdentifiedDate)
            .IsRequired()
            .HasColumnName("identified_date");

        builder.Property(pr => pr.MitigatedDate)
            .HasColumnName("mitigated_date");

        builder.Property(pr => pr.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(pr => pr.Impact)
            .HasPrecision(3, 1)
            .HasColumnName("impact");

        builder.Property(pr => pr.Probability)
            .HasPrecision(3, 1)
            .HasColumnName("probability");

        builder.Property(pr => pr.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(pr => pr.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(pr => pr.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(pr => pr.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(pr => pr.ProjectId)
            .HasDatabaseName("ix_project_risks_project_id");

        builder.HasIndex(pr => pr.RiskTypeId)
            .HasDatabaseName("ix_project_risks_risk_type_id");

        builder.HasIndex(pr => pr.RiskLevelId)
            .HasDatabaseName("ix_project_risks_risk_level_id");

        builder.HasIndex(pr => pr.IdentifiedDate)
            .HasDatabaseName("ix_project_risks_identified_date");

        // Relationships
        builder.HasOne(pr => pr.Project)
            .WithMany(p => p.ProjectRisks)
            .HasForeignKey(pr => pr.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.RiskType)
            .WithMany(rt => rt.ProjectRisks)
            .HasForeignKey(pr => pr.RiskTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.RiskLevel)
            .WithMany(rl => rl.ProjectRisks)
            .HasForeignKey(pr => pr.RiskLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}