using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;

namespace TeamTime.Application.Mappings;

public class TimeEntryMapper : IMapper<TimeEntry, TimeEntryDto>
{
    public TimeEntryDto Map(TimeEntry source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new TimeEntryDto
        {
            Id = source.Id,
            UserId = source.UserId,
            UserFullName = source.User?.FullName ?? string.Empty,
            ProjectId = source.ProjectId,
            ProjectName = source.Project?.Name ?? string.Empty,
            TaskId = source.TaskId,
            TaskName = source.Task?.Name,
            Date = source.Date,
            Hours = source.Hours,
            Description = source.Description,
            IsApproved = source.IsApproved,
            ApprovedById = source.ApprovedById,
            ApprovedByName = source.ApprovedBy?.FullName,
            ApprovedAt = source.ApprovedAt,
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public IEnumerable<TimeEntryDto> MapList(IEnumerable<TimeEntry> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(Map);
    }
}