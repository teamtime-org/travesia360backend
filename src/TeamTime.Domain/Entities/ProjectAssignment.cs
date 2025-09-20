using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ProjectAssignment : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AssignedById { get; private set; }
    public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual User AssignedBy { get; set; } = null!;

    // Constructors
    private ProjectAssignment() { } // EF Core constructor

    public ProjectAssignment(Guid projectId, Guid userId, Guid assignedById)
    {
        ProjectId = projectId;
        UserId = userId;
        AssignedById = assignedById;
        AssignedAt = DateTime.UtcNow;
        IsActive = true;
    }

    // Domain methods
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
}