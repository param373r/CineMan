namespace CineMan.Options;

public class JwtOptions
{
    public const string Jwt = "Jwt";
    public string AccessSecret { get; init; } = null!;
    public string RefreshSecret { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int AccessTokenLifeSpan { get; init; }
    public int RefreshTokenLifeSpan { get; init; }
}