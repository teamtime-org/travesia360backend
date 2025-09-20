using TeamTime.Domain.Entities;

namespace TeamTime.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    string? GetUserIdFromToken(string token);
    DateTime GetTokenExpiration(string token);
}