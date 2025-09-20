using Microsoft.AspNetCore.Identity;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;

namespace TeamTime.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? AreaId { get; set; }
    public Guid? JobTitleId { get; set; }
    public string? EmployeeCode { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // Navigation properties
    public virtual Area? Area { get; set; }
    public virtual JobTitle? JobTitle { get; set; }
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
    public virtual ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public virtual ICollection<TeamTimeTask> CreatedTasks { get; set; } = new List<TeamTimeTask>();
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public virtual ICollection<TimeEntry> ApprovedTimeEntries { get; set; } = new List<TimeEntry>();

    public string FullName => $"{FirstName} {LastName}";

    public bool CanManageProjects => Role == UserRole.ADMINISTRADOR || Role == UserRole.COORDINADOR;

    public bool CanApproveTimeEntries => Role == UserRole.ADMINISTRADOR || Role == UserRole.COORDINADOR;

    // Convert to Domain User
    public User ToDomainUser()
    {
        var user = new User(Email!, FirstName, LastName, Role, AreaId, JobTitleId);

        // Set private fields through reflection or domain methods
        typeof(User).GetProperty("Id")!.SetValue(user, Id);
        typeof(User).GetProperty("CreatedAt")!.SetValue(user, CreatedAt);
        typeof(User).GetProperty("UpdatedAt")!.SetValue(user, UpdatedAt);
        typeof(User).GetProperty("CreatedBy")!.SetValue(user, CreatedBy);
        typeof(User).GetProperty("LastModifiedBy")!.SetValue(user, LastModifiedBy);
        typeof(User).GetProperty("EmployeeCode")!.SetValue(user, EmployeeCode);
        typeof(User).GetProperty("PhoneNumber")!.SetValue(user, PhoneNumber);
        typeof(User).GetProperty("DateOfJoining")!.SetValue(user, DateOfJoining);
        typeof(User).GetProperty("EmailConfirmed")!.SetValue(user, EmailConfirmed);
        typeof(User).GetProperty("AccessFailedCount")!.SetValue(user, AccessFailedCount);
        typeof(User).GetProperty("LockoutEnd")!.SetValue(user, LockoutEnd);
        typeof(User).GetProperty("LockoutEnabled")!.SetValue(user, LockoutEnabled);

        if (!IsActive)
        {
            user.Deactivate();
        }

        return user;
    }

    // Create from Domain User
    public static ApplicationUser FromDomainUser(User user)
    {
        return new ApplicationUser
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.Email,
            NormalizedEmail = user.Email.ToUpperInvariant(),
            NormalizedUserName = user.Email.ToUpperInvariant(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            AreaId = user.AreaId,
            JobTitleId = user.JobTitleId,
            EmployeeCode = user.EmployeeCode,
            PhoneNumber = user.PhoneNumber,
            DateOfJoining = user.DateOfJoining,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            AccessFailedCount = user.AccessFailedCount,
            LockoutEnd = user.LockoutEnd,
            LockoutEnabled = user.LockoutEnabled,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            CreatedBy = user.CreatedBy,
            LastModifiedBy = user.LastModifiedBy,
            PasswordHash = user.PasswordHash
        };
    }
}