using TeamTime.Domain.Common;
using TeamTime.Domain.Enums;

namespace TeamTime.Domain.Entities;

public class Project : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid AreaId { get; private set; }
    public ProjectStatus Status { get; private set; } = ProjectStatus.ACTIVE;
    public Priority Priority { get; private set; } = Priority.MEDIUM;
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public decimal? EstimatedHours { get; private set; }
    public bool IsGeneral { get; private set; } = false;
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Area Area { get; set; } = null!;
    public virtual ICollection<ProjectAssignment> Assignments { get; set; } = new List<ProjectAssignment>();
    public virtual ICollection<TeamTimeTask> Tasks { get; set; } = new List<TeamTimeTask>();
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    // Constructors
    private Project() { } // EF Core constructor

    public Project(string name, Guid areaId, string? description = null, bool isGeneral = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AreaId = areaId;
        Description = description;
        IsGeneral = isGeneral;
        Status = ProjectStatus.ACTIVE;
        Priority = Priority.MEDIUM;
        IsActive = true;
    }

    // Domain methods
    public void UpdateDetails(string name, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date");

        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEstimatedHours(decimal? estimatedHours)
    {
        if (estimatedHours.HasValue && estimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive");

        EstimatedHours = estimatedHours;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ProjectStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = ProjectStatus.COMPLETED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ProjectStatus.CANCELLED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PutOnHold()
    {
        Status = ProjectStatus.ON_HOLD;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOverdue => EndDate.HasValue && EndDate < DateOnly.FromDateTime(DateTime.UtcNow) && Status != ProjectStatus.COMPLETED;

    public bool IsInProgress => Status == ProjectStatus.ACTIVE;
}