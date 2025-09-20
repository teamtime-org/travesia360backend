using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Tasks.Commands;

public class DeleteTaskCommand : ICommand<Result>
{
    public Guid Id { get; set; }
}