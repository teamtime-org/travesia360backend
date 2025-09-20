using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Users.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;
using TeamTime.Domain.Common;

namespace TeamTime.Application.Features.Users.Handlers;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, Result<TeamTime.Application.Common.PagedResult<UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<User, UserDto> _mapper;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper<User, UserDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<TeamTime.Application.Common.PagedResult<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _unitOfWork.Users.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                query.SearchTerm,
                query.Role,
                query.AreaId,
                query.IsActive,
                cancellationToken);

            var userDtos = users.Items.Select(_mapper.Map).ToList();

            var pagedResult = TeamTime.Application.Common.PagedResult<UserDto>.Create(
                userDtos,
                users.TotalCount,
                users.PageNumber,
                users.PageSize);

            return Result<TeamTime.Application.Common.PagedResult<UserDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<TeamTime.Application.Common.PagedResult<UserDto>>.Failure($"Error retrieving users: {ex.Message}");
        }
    }
}