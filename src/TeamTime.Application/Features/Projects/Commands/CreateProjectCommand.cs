using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Projects.Commands;

public class CreateProjectCommand : ICommand<Result<ProjectDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AreaId { get; set; }
    public Priority Priority { get; set; } = Priority.MEDIUM;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public bool IsGeneral { get; set; } = false;
    public Guid CreatedById { get; set; }
}