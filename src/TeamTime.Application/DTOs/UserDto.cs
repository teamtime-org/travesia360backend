using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? AreaId { get; set; }
    public Guid? JobTitleId { get; set; }
    public string? EmployeeCode { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }

    // Navigation properties
    public AreaDto? Area { get; set; }
    public JobTitleDto? JobTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}