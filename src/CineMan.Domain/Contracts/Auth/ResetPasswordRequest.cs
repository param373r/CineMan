namespace CineMan.Domain.Contracts.Auth;

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword
);
