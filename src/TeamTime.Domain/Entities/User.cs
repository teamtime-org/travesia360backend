using TeamTime.Domain.Common;
using TeamTime.Domain.Enums;

namespace TeamTime.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    // Authentication & Identity fields
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool EmailConfirmed { get; private set; } = false;
    public int AccessFailedCount { get; private set; } = 0;
    public DateTime? LockoutEnd { get; private set; }
    public bool LockoutEnabled { get; private set; } = true;

    // Profile & Work Information
    public Guid? AreaId { get; private set; }
    public Guid? JobTitleId { get; private set; }
    public string? EmployeeCode { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime? DateOfJoining { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Import & Integration fields (for Person functionality)
    public string? FullName { get; internal set; } // For imports that have full name instead of first/last
    public bool IsImported { get; private set; } = false; // Track if user was imported from Excel

    // Navigation properties
    public virtual Area? Area { get; set; }
    public virtual JobTitle? JobTitle { get; set; }

    // Current system relationships
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

    public string GetDisplayName() => !string.IsNullOrEmpty(FullName) ? FullName : $"{FirstName} {LastName}";

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

    // Import/Person compatibility methods
    public static User CreateFromImport(string fullName, string? email = null, string? phone = null, string? employeeCode = null)
    {
        var user = new User();
        user.SetFromFullName(fullName);
        user.Email = email?.Trim() ?? "";
        user.PhoneNumber = phone?.Trim();
        user.EmployeeCode = employeeCode?.Trim();
        user.FullName = fullName?.Trim();
        user.IsImported = true;
        user.IsActive = true;
        user.Role = UserRole.COLABORADOR; // Default role for imported users
        user.EmailConfirmed = false;
        user.LockoutEnabled = true;
        return user;
    }

    public void UpdateFromImport(string fullName, string? email = null, string? phone = null, string? employeeCode = null)
    {
        SetFromFullName(fullName);
        if (email != null) Email = email.Trim();
        if (phone != null) PhoneNumber = phone.Trim();
        if (employeeCode != null) EmployeeCode = employeeCode.Trim();
        FullName = fullName?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetFromFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            FirstName = parts[0];
            LastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";
        }
    }

    // Name helper methods (from Person entity)
    public string GetFirstName() => FirstName;
    public string GetLastName() => LastName;

    // Static helper for normalization (from Person entity)
    public static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        // Remove extra spaces, semicolons and IDs from SharePoint
        var normalized = name.Trim()
            .Replace(";#", " ")
            .Replace("#", "")
            .Replace("  ", " ");

        // Extract only the name part if there are numbers at the end
        var parts = normalized.Split(' ');
        var cleanParts = new List<string>();

        foreach (var part in parts)
        {
            if (!int.TryParse(part, out _))
            {
                cleanParts.Add(part);
            }
        }

        return string.Join(" ", cleanParts).Trim();
    }
}