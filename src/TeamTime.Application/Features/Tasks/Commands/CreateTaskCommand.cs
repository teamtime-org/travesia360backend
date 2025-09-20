using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Tasks.Commands;

public class CreateTaskCommand : ICommand<Result<TaskDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Priority Priority { get; set; } = Priority.MEDIUM;
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}