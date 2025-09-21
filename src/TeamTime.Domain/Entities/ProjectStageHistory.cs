using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ProjectStageHistory : BaseEntity, IAggregateRoot
{
    public Guid ProjectId { get; private set; }
    public Guid ProjectStageId { get; private set; }
    public DateTime ChangeDate { get; private set; }
    public string? Notes { get; private set; }
    public string? ChangedBy { get; private set; } // Could be user email or system
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual ProjectStage ProjectStage { get; set; } = null!;

    // Constructors
    private ProjectStageHistory() { } // EF Core constructor

    public ProjectStageHistory(
        Guid projectId,
        Guid projectStageId,
        DateTime? changeDate = null,
        string? notes = null,
        string? changedBy = null)
    {
        ProjectId = projectId;
        ProjectStageId = projectStageId;
        ChangeDate = changeDate ?? DateTime.UtcNow;
        Notes = notes?.Trim();
        ChangedBy = changedBy?.Trim();
        IsActive = true;
    }

    // Domain methods
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business logic methods
    public TimeSpan GetTimeInStage(DateTime? endDate = null)
    {
        var end = endDate ?? DateTime.UtcNow;
        return end - ChangeDate;
    }
}