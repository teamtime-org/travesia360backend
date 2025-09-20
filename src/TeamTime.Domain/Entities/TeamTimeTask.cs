using TeamTime.Domain.Common;
using TeamTime.Domain.Enums;

namespace TeamTime.Domain.Entities;

public class TeamTimeTask : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid ProjectId { get; private set; }
    public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.TODO;
    public Priority Priority { get; private set; } = Priority.MEDIUM;
    public DateOnly? DueDate { get; private set; }
    public decimal? EstimatedHours { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    // Constructors
    private TeamTimeTask() { } // EF Core constructor

    public TeamTimeTask(string name, Guid projectId, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProjectId = projectId;
        Description = description;
        Status = Enums.TaskStatus.TODO;
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

    public void UpdateStatus(Enums.TaskStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDueDate(DateOnly? dueDate)
    {
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEstimatedHours(decimal? estimatedHours)
    {
        if (estimatedHours.HasValue && estimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive");

        EstimatedHours = estimatedHours;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        Status = Enums.TaskStatus.IN_PROGRESS;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = Enums.TaskStatus.DONE;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SendToReview()
    {
        Status = Enums.TaskStatus.REVIEW;
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

    public bool IsOverdue => DueDate.HasValue && DueDate < DateOnly.FromDateTime(DateTime.UtcNow) && Status != Enums.TaskStatus.DONE;

    public bool IsCompleted => Status == Enums.TaskStatus.DONE;

    public bool IsInProgress => Status == Enums.TaskStatus.IN_PROGRESS;
}