using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Domain.Entities;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");

        builder.Property(p => p.AreaId)
            .HasColumnName("area_id");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.ProjectStatus.ACTIVE)
            .HasColumnName("status");

        builder.Property(p => p.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.Priority.MEDIUM)
            .HasColumnName("priority");

        builder.Property(p => p.StartDate)
            .HasColumnName("start_date");

        builder.Property(p => p.EndDate)
            .HasColumnName("end_date");

        builder.Property(p => p.EstimatedHours)
            .HasColumnType("decimal(8,2)")
            .HasColumnName("estimated_hours");

        builder.Property(p => p.IsGeneral)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_general");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        // Import system fields
        builder.Property(p => p.ProjectCode)
            .HasMaxLength(50)
            .HasColumnName("project_code");

        builder.Property(p => p.ClientId)
            .HasColumnName("client_id");

        builder.Property(p => p.SegmentId)
            .HasColumnName("segment_id");

        builder.Property(p => p.ProjectStageId)
            .HasColumnName("project_stage_id");

        builder.Property(p => p.ServiceTypeId)
            .HasColumnName("service_type_id");

        builder.Property(p => p.ContractTypeId)
            .HasColumnName("contract_type_id");

        builder.Property(p => p.BusinessLineId)
            .HasColumnName("business_line_id");

        // Financial Information
        builder.Property(p => p.ContractValue)
            .HasPrecision(18, 2)
            .HasColumnName("contract_value");

        builder.Property(p => p.ActualHours)
            .HasPrecision(18, 2)
            .HasColumnName("actual_hours");

        builder.Property(p => p.Progress)
            .HasPrecision(5, 2)
            .HasColumnName("progress");

        // Extended Dates
        builder.Property(p => p.PlannedEndDate)
            .HasColumnName("planned_end_date");

        builder.Property(p => p.ActualEndDate)
            .HasColumnName("actual_end_date");

        builder.Property(p => p.ContractDate)
            .HasColumnName("contract_date");

        // Additional Control fields
        builder.Property(p => p.IsConfidential)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_confidential");

        builder.Property(p => p.Notes)
            .HasMaxLength(2000)
            .HasColumnName("notes");

        builder.Property(p => p.Observations)
            .HasMaxLength(2000)
            .HasColumnName("observations");

        // Import tracking
        builder.Property(p => p.ExcelFileName)
            .HasMaxLength(255)
            .HasColumnName("excel_file_name");

        builder.Property(p => p.ExcelRowNumber)
            .HasColumnName("excel_row_number");

        builder.Property(p => p.LastImportDate)
            .HasColumnName("last_import_date");

        builder.Property(p => p.IsImported)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_imported");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(p => p.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(p => p.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Indexes
        builder.HasIndex(p => p.ProjectCode)
            .IsUnique()
            .HasFilter("project_code IS NOT NULL")
            .HasDatabaseName("ix_projects_project_code");

        builder.HasIndex(p => p.ClientId)
            .HasDatabaseName("ix_projects_client_id");

        builder.HasIndex(p => p.SegmentId)
            .HasDatabaseName("ix_projects_segment_id");

        builder.HasIndex(p => p.ProjectStageId)
            .HasDatabaseName("ix_projects_project_stage_id");

        builder.HasIndex(p => p.IsImported)
            .HasDatabaseName("ix_projects_is_imported");

        builder.HasIndex(p => new { p.ExcelFileName, p.ExcelRowNumber })
            .HasDatabaseName("ix_projects_excel_import");

        // Relationships - Legacy system
        builder.HasOne(p => p.Area)
            .WithMany(a => a.Projects)
            .HasForeignKey(p => p.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Assignments)
            .WithOne(pa => pa.Project)
            .HasForeignKey(pa => pa.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.TimeEntries)
            .WithOne(te => te.Project)
            .HasForeignKey(te => te.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - Import system
        builder.HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Segment)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProjectStage)
            .WithMany(ps => ps.Projects)
            .HasForeignKey(p => p.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ServiceType)
            .WithMany(st => st.Projects)
            .HasForeignKey(p => p.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ContractType)
            .WithMany(ct => ct.Projects)
            .HasForeignKey(p => p.ContractTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.BusinessLine)
            .WithMany(bl => bl.Projects)
            .HasForeignKey(p => p.BusinessLineId)
            .OnDelete(DeleteBehavior.Restrict);

        // Collections for import system
        builder.HasMany(p => p.ProjectRisks)
            .WithOne(pr => pr.Project)
            .HasForeignKey(pr => pr.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProjectStageHistory)
            .WithOne(psh => psh.Project)
            .HasForeignKey(psh => psh.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}