using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;

namespace TeamTime.Application.Mappings;

public class AreaMapper : IMapper<Area, AreaDto>
{
    public AreaDto Map(Area source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new AreaDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Color = source.Color,
            IsActive = source.IsActive,
            UserCount = source.Users?.Count ?? 0,
            ProjectCount = source.Projects?.Count ?? 0,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public IEnumerable<AreaDto> MapList(IEnumerable<Area> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(Map);
    }
}