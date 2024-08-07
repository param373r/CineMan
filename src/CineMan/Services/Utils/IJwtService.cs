using System.Security.Claims;

namespace CineMan.Services.Utils;

public interface IJwtService
{
    string GenerateRefreshToken(string userId);
    string GenerateAccessToken(string userId);
    ClaimsPrincipal? ValidateRefreshToken(string token);
    ClaimsPrincipal? ValidateAccessToken(string token);
}