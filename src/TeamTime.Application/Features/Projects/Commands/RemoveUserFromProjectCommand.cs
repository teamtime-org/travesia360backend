using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Projects.Commands;

public class RemoveUserFromProjectCommand : ICommand<Result<Unit>>
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid RemovedById { get; set; }
}