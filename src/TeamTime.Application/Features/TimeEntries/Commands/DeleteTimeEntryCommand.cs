using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.TimeEntries.Commands;

public class DeleteTimeEntryCommand : ICommand<Result>
{
    public Guid Id { get; set; }
}