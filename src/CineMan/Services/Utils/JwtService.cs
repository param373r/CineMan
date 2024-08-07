using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CineMan.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CineMan.Services.Utils;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private readonly ILogger<JwtService> _logger;
    public JwtService(IOptions<JwtOptions> options, ILogger<JwtService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string GenerateRefreshToken(string userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim("allowLogin", "false")
        };

        return GenerateToken(claims, _options.RefreshTokenLifeSpan, _options.RefreshSecret);
    }

    public string GenerateAccessToken(string userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim("allowLogin", "true")
        };

        return GenerateToken(claims, _options.AccessTokenLifeSpan, _options.AccessSecret);
    }

    public ClaimsPrincipal? ValidateRefreshToken(string token)
    {
        return ValidateToken(token, _options.RefreshSecret);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        return ValidateToken(token, _options.AccessSecret);
    }

    private ClaimsPrincipal? ValidateToken(string token, string secret)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception)
        {
            _logger.LogError("Failed to validate token");
            return null;
        }
    }

    private string GenerateToken(Claim[] claims, int lifeSpan, string secret)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(lifeSpan),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
