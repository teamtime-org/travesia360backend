using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.TimeEntries.Commands;

public class ApproveTimeEntryCommand : ICommand<Result<TimeEntryDto>>
{
    public Guid Id { get; set; }
    public Guid ApprovedById { get; set; }
}