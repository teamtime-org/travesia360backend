using System.ComponentModel.DataAnnotations;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class RegisterUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public Guid? AreaId { get; set; }

    public Guid? JobTitleId { get; set; }

    [MaxLength(50)]
    public string? EmployeeCode { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfJoining { get; set; }
}

public class UpdateUserProfileDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? EmployeeCode { get; set; }

    public Guid? AreaId { get; set; }

    public Guid? JobTitleId { get; set; }

    public DateTime? DateOfJoining { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}