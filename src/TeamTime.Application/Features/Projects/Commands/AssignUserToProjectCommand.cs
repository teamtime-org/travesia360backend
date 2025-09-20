using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Projects.Commands;

public class AssignUserToProjectCommand : ICommand<Result<Unit>>
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid AssignedById { get; set; }
}