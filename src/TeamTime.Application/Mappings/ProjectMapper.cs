using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;

namespace TeamTime.Application.Mappings;

public class ProjectMapper : IMapper<Project, ProjectDto>
{
    public ProjectDto Map(Project source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new ProjectDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            AreaId = source.AreaId ?? Guid.Empty,
            AreaName = source.Area?.Name ?? string.Empty,
            Status = source.Status,
            Priority = source.Priority,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            EstimatedHours = source.EstimatedHours,
            IsGeneral = source.IsGeneral,
            IsActive = source.IsActive,
            IsOverdue = source.IsOverdue,
            AssignedUserCount = source.Assignments?.Count(a => a.IsActive) ?? 0,
            TaskCount = source.Tasks?.Count ?? 0,
            TotalLoggedHours = source.TimeEntries?.Sum(te => te.Hours) ?? 0,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public IEnumerable<ProjectDto> MapList(IEnumerable<Project> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(Map);
    }
}