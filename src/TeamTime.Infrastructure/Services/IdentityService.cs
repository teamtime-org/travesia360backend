using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamTime.Application.DTOs;
using TeamTime.Application.Services;
using TeamTime.Domain.Enums;
using TeamTime.Infrastructure.Identity;
using TeamTime.Infrastructure.Data;

namespace TeamTime.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials or inactive user");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedAccessException("Account is locked out");
            }
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var domainUser = user.ToDomainUser();
        var token = _tokenService.GenerateAccessToken(domainUser);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Update refresh token in database (you might want to store this)
        user.SecurityStamp = refreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.SecurityStamp == refreshToken && u.IsActive);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var domainUser = user.ToDomainUser();
        var newToken = _tokenService.GenerateAccessToken(domainUser);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.SecurityStamp = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            User = MapToUserDto(user)
        };
    }

    public async Task<UserDto> RegisterUserAsync(string email, string password, string firstName, string lastName,
        UserRole role, Guid? areaId = null, Guid? jobTitleId = null, Guid? createdBy = null)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            AreaId = areaId,
            JobTitleId = jobTitleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            IsActive = true,
            EmailConfirmed = true // Auto-confirm for admin created users
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // Add user to role
        var roleName = role.ToString();
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        return MapToUserDto(user);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Area)
            .Include(u => u.JobTitle)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return MapToUserDto(user);
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.Area)
            .Include(u => u.JobTitle)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return MapToUserDto(user);
    }

    public async Task<UserDto> UpdateUserProfileAsync(Guid userId, string firstName, string lastName,
        string? phoneNumber = null, string? employeeCode = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phoneNumber;
        user.EmployeeCode = employeeCode;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return MapToUserDto(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword, Guid? resetBy = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (result.Succeeded)
        {
            user.LastModifiedBy = resetBy;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        return result.Succeeded;
    }

    public async Task<bool> LockUserAsync(Guid userId, DateTime? lockoutEnd = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd ?? DateTimeOffset.UtcNow.AddYears(100));
        return result.Succeeded;
    }

    public async Task<bool> UnlockUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        if (result.Succeeded)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
        }
        return result.Succeeded;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Area)
            .Include(u => u.JobTitle)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        return users.Select(MapToUserDto);
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<IList<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new List<string>();
        }

        return await _userManager.GetRolesAsync(user);
    }

    private static UserDto MapToUserDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
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
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Area = user.Area != null ? new AreaDto
            {
                Id = user.Area.Id,
                Name = user.Area.Name,
                Description = user.Area.Description,
                Color = user.Area.Color,
                IsActive = user.Area.IsActive
            } : null
        };
    }
}