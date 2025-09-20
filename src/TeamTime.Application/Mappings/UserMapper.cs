using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;

namespace TeamTime.Application.Mappings;

public class UserMapper : IMapper<User, UserDto>
{
    public UserDto Map(User source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new UserDto
        {
            Id = source.Id,
            Email = source.Email,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FullName = source.FullName,
            Role = source.Role,
            AreaId = source.AreaId,
            JobTitleId = source.JobTitleId,
            EmployeeCode = source.EmployeeCode,
            PhoneNumber = source.PhoneNumber,
            DateOfJoining = source.DateOfJoining,
            IsActive = source.IsActive,
            EmailConfirmed = source.EmailConfirmed,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            Area = source.Area != null ? new AreaDto
            {
                Id = source.Area.Id,
                Name = source.Area.Name,
                Description = source.Area.Description,
                Color = source.Area.Color,
                IsActive = source.Area.IsActive
            } : null,
            JobTitle = source.JobTitle != null ? new JobTitleDto
            {
                Id = source.JobTitle.Id,
                Name = source.JobTitle.Name,
                Description = source.JobTitle.Description,
                Department = source.JobTitle.Department,
                Level = source.JobTitle.Level,
                IsActive = source.JobTitle.IsActive
            } : null
        };
    }

    public IEnumerable<UserDto> MapList(IEnumerable<User> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(Map);
    }
}