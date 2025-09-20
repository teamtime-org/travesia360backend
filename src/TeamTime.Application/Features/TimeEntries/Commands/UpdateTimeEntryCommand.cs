using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.TimeEntries.Commands;

public class UpdateTimeEntryCommand : ICommand<Result<TimeEntryDto>>
{
    public Guid Id { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public Guid? TaskId { get; set; }
}