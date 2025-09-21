using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectStageHistoryConfiguration : IEntityTypeConfiguration<ProjectStageHistory>
{
    public void Configure(EntityTypeBuilder<ProjectStageHistory> builder)
    {
        builder.ToTable("project_stage_history");

        builder.HasKey(psh => psh.Id);

        builder.Property(psh => psh.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(psh => psh.ProjectId)
            .IsRequired()
            .HasColumnName("project_id");

        builder.Property(psh => psh.ProjectStageId)
            .IsRequired()
            .HasColumnName("project_stage_id");

        builder.Property(psh => psh.ChangeDate)
            .IsRequired()
            .HasColumnName("change_date");

        builder.Property(psh => psh.Notes)
            .HasMaxLength(1000)
            .HasColumnName("notes");

        builder.Property(psh => psh.ChangedBy)
            .HasMaxLength(100)
            .HasColumnName("changed_by");

        builder.Property(psh => psh.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(psh => psh.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(psh => psh.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(psh => psh.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(psh => psh.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(psh => psh.ProjectId)
            .HasDatabaseName("ix_project_stage_history_project_id");

        builder.HasIndex(psh => psh.ProjectStageId)
            .HasDatabaseName("ix_project_stage_history_project_stage_id");

        builder.HasIndex(psh => psh.ChangeDate)
            .HasDatabaseName("ix_project_stage_history_change_date");

        builder.HasIndex(psh => new { psh.ProjectId, psh.ChangeDate })
            .HasDatabaseName("ix_project_stage_history_project_change_date");

        // Relationships
        builder.HasOne(psh => psh.Project)
            .WithMany(p => p.ProjectStageHistory)
            .HasForeignKey(psh => psh.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(psh => psh.ProjectStage)
            .WithMany(ps => ps.StageHistories)
            .HasForeignKey(psh => psh.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}