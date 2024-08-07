namespace CineMan.Domain.Contracts.Auth;

public record ChangePasswordRequest(
    string OldPassword,
    string NewPassword
);