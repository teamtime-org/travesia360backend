using TeamTime.Domain.Common;
using TeamTime.Domain.Enums;

namespace TeamTime.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Guid? AreaId { get; private set; }
    public Guid? JobTitleId { get; private set; }
    public string? EmployeeCode { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime? DateOfJoining { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool EmailConfirmed { get; private set; } = false;
    public int AccessFailedCount { get; private set; } = 0;
    public DateTime? LockoutEnd { get; private set; }
    public bool LockoutEnabled { get; private set; } = true;

    // Navigation properties
    public virtual Area? Area { get; set; }
    public virtual JobTitle? JobTitle { get; set; }
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
    public virtual ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public virtual ICollection<TeamTimeTask> CreatedTasks { get; set; } = new List<TeamTimeTask>();
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public virtual ICollection<TimeEntry> ApprovedTimeEntries { get; set; } = new List<TimeEntry>();

    // Constructors
    private User() { } // EF Core constructor

    public User(string email, string firstName, string lastName, UserRole role, Guid? areaId = null, Guid? jobTitleId = null)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Role = role;
        AreaId = areaId;
        JobTitleId = jobTitleId;
        IsActive = true;
        EmailConfirmed = false;
        LockoutEnabled = true;
    }

    // Domain methods
    public void UpdateProfile(string firstName, string lastName, string? phoneNumber = null, string? employeeCode = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        PhoneNumber = phoneNumber;
        EmployeeCode = employeeCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToArea(Guid areaId)
    {
        AreaId = areaId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFromArea()
    {
        AreaId = null;
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

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";

    public bool CanManageProjects => Role == UserRole.ADMINISTRADOR || Role == UserRole.COORDINADOR;

    public bool CanApproveTimeEntries => Role == UserRole.ADMINISTRADOR || Role == UserRole.COORDINADOR;

    public void AssignJobTitle(Guid jobTitleId)
    {
        JobTitleId = jobTitleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveJobTitle()
    {
        JobTitleId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetAccessFailedCount()
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementAccessFailedCount()
    {
        AccessFailedCount++;
        if (AccessFailedCount >= 5 && LockoutEnabled)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
}