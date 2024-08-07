namespace CineMan.Domain.Contracts.Auth;

public record RegisterUserRequest(
    string Email,
    string Password
);