using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;

namespace TeamTime.Application.Mappings;

public class TaskMapper : IMapper<TeamTimeTask, TaskDto>
{
    public TaskDto Map(TeamTimeTask source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new TaskDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            ProjectId = source.ProjectId,
            ProjectName = source.Project?.Name ?? string.Empty,
            Status = source.Status,
            Priority = source.Priority,
            DueDate = source.DueDate,
            EstimatedHours = source.EstimatedHours,
            IsActive = source.IsActive,
            IsOverdue = source.IsOverdue,
            IsCompleted = source.IsCompleted,
            IsInProgress = source.IsInProgress,
            TotalLoggedHours = source.TimeEntries?.Sum(te => te.Hours) ?? 0,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public IEnumerable<TaskDto> MapList(IEnumerable<TeamTimeTask> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(Map);
    }
}