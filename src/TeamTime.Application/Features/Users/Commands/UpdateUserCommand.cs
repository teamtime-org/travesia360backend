using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Users.Commands;

public class UpdateUserCommand : ICommand<Result<UserDto>>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? AreaId { get; set; }
}