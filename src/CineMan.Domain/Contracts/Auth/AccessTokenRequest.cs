namespace CineMan.Domain.Contracts.Auth;

public record AccessTokenRequest(
    string RefreshToken
);