using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.TimeEntries.Queries;

public class GetTimeEntryByIdQuery : IQuery<Result<TimeEntryDto>>
{
    public Guid Id { get; set; }
}