using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

// Unified ProjectAssignment entity for both current system and import system
public class ProjectAssignment : BaseEntity, IAggregateRoot
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }

    // Legacy fields (for existing system)
    public Guid? AssignedById { get; private set; }
    public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

    // New fields (for import system)
    public Guid? ProjectRoleId { get; private set; }
    public DateTime? AssignedDate { get; private set; }
    public DateTime? UnassignedDate { get; private set; }
    public string? Notes { get; private set; }

    public bool IsActive { get; private set; } = true;
    public bool IsImported { get; private set; } = false; // Track if assignment came from import

    // Navigation properties
    public virtual Project? Project { get; set; } // Unified project entity
    public virtual User User { get; set; } = null!;
    public virtual User? AssignedBy { get; set; }
    public virtual ProjectRole? ProjectRole { get; set; }

    // Constructors
    private ProjectAssignment() { } // EF Core constructor

    // Constructor for existing system
    public ProjectAssignment(Guid projectId, Guid userId, Guid assignedById)
    {
        ProjectId = projectId;
        UserId = userId;
        AssignedById = assignedById;
        AssignedAt = DateTime.UtcNow;
        IsActive = true;
        IsImported = false;
    }

    // Constructor for import system
    public static ProjectAssignment CreateForImport(Guid projectId, Guid userId, Guid projectRoleId, DateTime? assignedDate = null, string? notes = null)
    {
        return new ProjectAssignment
        {
            ProjectId = projectId,
            UserId = userId,
            ProjectRoleId = projectRoleId,
            AssignedDate = assignedDate ?? DateTime.UtcNow,
            AssignedAt = assignedDate ?? DateTime.UtcNow, // Keep legacy field for compatibility
            Notes = notes?.Trim(),
            IsActive = true,
            IsImported = true
        };
    }

    // Domain methods (legacy system)
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Domain methods (import system)
    public void UpdateRole(Guid projectRoleId, string? notes = null)
    {
        ProjectRoleId = projectRoleId;
        if (notes != null) Notes = notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unassign(DateTime? unassignedDate = null)
    {
        UnassignedDate = unassignedDate ?? DateTime.UtcNow;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business logic methods
    public bool IsActiveOn(DateTime date)
    {
        if (!IsImported)
            return IsActive; // Legacy behavior

        return IsActive &&
               (AssignedDate?.Date <= date.Date || AssignedAt.Date <= date.Date) &&
               (UnassignedDate == null || UnassignedDate.Value.Date > date.Date);
    }

    public TimeSpan GetAssignmentDuration()
    {
        var endDate = UnassignedDate ?? DateTime.UtcNow;
        var startDate = AssignedDate ?? AssignedAt;
        return endDate - startDate;
    }
}