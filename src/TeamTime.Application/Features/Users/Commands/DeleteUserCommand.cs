using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Users.Commands;

public class DeleteUserCommand : ICommand<Result>
{
    public Guid Id { get; set; }
}