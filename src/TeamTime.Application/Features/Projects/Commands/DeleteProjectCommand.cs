using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Projects.Commands;

public class DeleteProjectCommand : ICommand<Result<Unit>>
{
    public Guid Id { get; set; }
    public Guid DeletedById { get; set; }
}