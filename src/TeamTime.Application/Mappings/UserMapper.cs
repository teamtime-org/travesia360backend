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
            AreaName = source.Area?.Name,
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public IEnumerable<UserDto> MapList(IEnumerable<User> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(Map);
    }
}