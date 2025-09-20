using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.TimeEntries.Commands;

public class CreateTimeEntryCommand : ICommand<Result<TimeEntryDto>>
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? TaskId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
}