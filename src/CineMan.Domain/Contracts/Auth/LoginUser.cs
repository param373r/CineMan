namespace CineMan.Domain.Contracts.Auth;

public record LoginUserRequest(
    string Email,
    string Password
);
