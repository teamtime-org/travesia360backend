using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Auth.Commands;

public class RefreshTokenCommand : ICommand<Result<AuthResponseDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}