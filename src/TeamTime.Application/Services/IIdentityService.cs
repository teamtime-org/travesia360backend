using TeamTime.Application.DTOs;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Services;

public interface IIdentityService
{
    Task<AuthResponseDto> LoginAsync(string email, string password);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto> RegisterUserAsync(string email, string password, string firstName, string lastName,
        UserRole role, Guid? areaId = null, Guid? jobTitleId = null, Guid? createdBy = null);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto> UpdateUserProfileAsync(Guid userId, string firstName, string lastName,
        string? phoneNumber = null, string? employeeCode = null);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(Guid userId, string newPassword, Guid? resetBy = null);
    Task<bool> LockUserAsync(Guid userId, DateTime? lockoutEnd = null);
    Task<bool> UnlockUserAsync(Guid userId);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<bool> ActivateUserAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<bool> IsInRoleAsync(Guid userId, string roleName);
    Task<IList<string>> GetUserRolesAsync(Guid userId);
}