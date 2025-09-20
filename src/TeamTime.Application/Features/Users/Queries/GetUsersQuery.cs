using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Features.Users.Queries;

public class GetUsersQuery : IQuery<Result<TeamTime.Application.Common.PagedResult<UserDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public UserRole? Role { get; set; }
    public Guid? AreaId { get; set; }
    public bool? IsActive { get; set; }
}