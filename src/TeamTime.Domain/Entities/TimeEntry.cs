using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? TaskId { get; private set; }
    public Guid TimePeriodId { get; private set; }
    public DateOnly Date { get; private set; }
    public decimal Hours { get; private set; }
    public string? Description { get; private set; }
    public bool IsApproved { get; private set; } = false;
    public Guid? ApprovedById { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual TeamTimeTask? Task { get; set; }
    public virtual TimePeriod TimePeriod { get; set; } = null!;

    // Constructors
    private TimeEntry() { } // EF Core constructor

    public TimeEntry(Guid userId, Guid projectId, Guid timePeriodId, DateOnly date, decimal hours, string? description = null, Guid? taskId = null)
    {
        if (hours <= 0)
            throw new ArgumentException("Hours must be positive");

        if (hours > 24)
            throw new ArgumentException("Hours cannot exceed 24 per day");

        UserId = userId;
        ProjectId = projectId;
        TimePeriodId = timePeriodId;
        TaskId = taskId;
        Date = date;
        Hours = hours;
        Description = description;
        IsApproved = false;
        IsActive = true;
    }

    // Domain methods
    public void UpdateHours(decimal hours)
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved time entry");

        if (hours <= 0)
            throw new ArgumentException("Hours must be positive");

        if (hours > 24)
            throw new ArgumentException("Hours cannot exceed 24 per day");

        Hours = hours;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved time entry");

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTask(Guid? taskId)
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved time entry");

        TaskId = taskId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(Guid approvedById)
    {
        IsApproved = true;
        ApprovedById = approvedById;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        ApprovedById = null;
        ApprovedAt = null;
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

    public void UpdateTimePeriod(Guid timePeriodId)
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved time entry");

        TimePeriodId = timePeriodId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ValidateWithinTimePeriod(TimePeriod timePeriod)
    {
        if (!timePeriod.ContainsDate(Date.ToDateTime(TimeOnly.MinValue)))
        {
            throw new InvalidOperationException($"Time entry date {Date} is not within the time period {timePeriod.Name} ({timePeriod.StartDate:yyyy-MM-dd} to {timePeriod.EndDate:yyyy-MM-dd})");
        }
    }

    public bool IsWithinActivePeriod(TimePeriod timePeriod)
    {
        return timePeriod.IsActive && timePeriod.ContainsDate(Date.ToDateTime(TimeOnly.MinValue));
    }

    public bool CanBeModified => !IsApproved && IsActive;

    public bool IsPending => !IsApproved && IsActive;
}