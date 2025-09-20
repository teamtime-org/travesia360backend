using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.TimeEntries.Commands;

public class RejectTimeEntryCommand : ICommand<Result<TimeEntryDto>>
{
    public Guid Id { get; set; }
}