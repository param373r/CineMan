using CineMan.Domain.Contracts.Auth;
using CineMan.Errors;

namespace CineMan.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterUserRequest request);
    Task<Result<TokenResponse>> LoginAsync(LoginUserRequest request);
    Task<Result<TokenResponse>> RefreshAccessTokenAsync(AccessTokenRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request, Guid id);
    Task<Result> ChangeEmailAsync(ChangeEmailRequest request, Guid id);
    Task<Result> ConfirmEmailAsync(string token);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}