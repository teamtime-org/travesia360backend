using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Users.Queries;

public class GetUserByIdQuery : IQuery<Result<UserDto?>>
{
    public Guid Id { get; set; }
}