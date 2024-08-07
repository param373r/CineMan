using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CineMan.Domain.Contracts.Auth;
using CineMan.Domain.Models.Users;
using CineMan.Errors;
using CineMan.Errors.ErrorConstants;
using CineMan.Options;
using CineMan.Persistence.Data;
using CineMan.Services.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CineMan.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly AuthOptions _options;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;
    public AuthService(
        AppDbContext dbContext,
        IOptions<AuthOptions> options,
        IJwtService jwtService,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _jwtService = jwtService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            _logger.LogError("Email or password is empty");
            return Result.Failure<TokenResponse>(UserErrors.CredentialsInvalid);
        }

        _logger.LogInformation("Checking whether user exists in the database");
        var user = await _dbContext.Users
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            _logger.LogError("User not found");
            return Result.Failure<TokenResponse>(UserErrors.CredentialsInvalid);
        }

        _logger.LogInformation("Checking whether password is correct");
        var result = VerifyPassword(request.Password, user.Password);
        if (!result)
        {
            _logger.LogError("Incorrect password");
            return Result.Failure<TokenResponse>(UserErrors.CredentialsInvalid);
        }

        _logger.LogInformation("Generating refresh token");
        string token = _jwtService.GenerateRefreshToken(user.Id.ToString());
        return Result.Success(new TokenResponse(token));
    }

    public async Task<Result<TokenResponse>> RefreshAccessTokenAsync(AccessTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            _logger.LogError("Refresh token is empty");
            return Result.Failure<TokenResponse>(UserErrors.InvalidRefreshToken);
        }

        _logger.LogInformation("Validating refresh token");
        var principal = _jwtService.ValidateRefreshToken(request.RefreshToken);
        if (principal is null)
        {
            _logger.LogError("Invalid refresh token");
            return Result.Failure<TokenResponse>(UserErrors.InvalidRefreshToken);
        }

        _logger.LogInformation("Checking whether user exists in the database");
        var user = await _dbContext.Users.FindAsync(Guid.Parse(principal.Identity!.Name!));
        if (user == null)
        {
            _logger.LogError("User not found");
            return Result.Failure<TokenResponse>(UserErrors.InvalidRefreshToken);
        }

        _logger.LogInformation("Generating access token");
        var token = _jwtService.GenerateAccessToken(user.Id.ToString());
        return Result.Success(new TokenResponse(token));
    }

    public async Task<Result> RegisterAsync(RegisterUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            _logger.LogError("Email or password is empty");
            return Result.Failure(UserErrors.EmptyCredentials);
        }
        if (!IsValidEmail(request.Email))
        {
            _logger.LogError("Invalid email format");
            return Result.Failure(UserErrors.EmailFormatInvalid);
        }

        _logger.LogInformation("Checking whether user exists in the database");
        var userExists = await IsExistingUser(request.Email);
        if (userExists.IsSuccess && userExists.GetValue())
        {
            _logger.LogError("User already exists");
            return Result.Failure(UserErrors.UserAlreadyExists);
        }
        else if (IsExistingUser(request.Email).Result.Error == UserErrors.EmailNotConfirmed)
        {
            _logger.LogError("Email not confirmed and user exists");
            return Result.Failure(UserErrors.EmailNotConfirmedAndUserExists);
        }

        _logger.LogInformation("Checking whether password meets the requirements");
        if (!IsValidPassword(request.Password))
        {
            _logger.LogError("Password does not meet the requirements");
            return Result.Failure(UserErrors.PasswordPolicyNotMet);
        }

        _logger.LogInformation("Hashing the password");
        string hashedPassword = HashPassword(request.Password);

        _logger.LogInformation("Generating confirmation token");
        string confirmationToken = GenerateConfirmationToken();

        User user = new User
        {
            Email = request.Email,
            ConfirmationToken = confirmationToken,
            Password = hashedPassword
        };

        _logger.LogInformation("Adding user to the database");
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Sending confirmation email to the user");
        await _emailService.SendEmailConfirmationEmail(request.Email, confirmationToken);

        _logger.LogInformation("User registered successfully");
        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Token is empty");
            return Result.Failure(UserErrors.TokenInvalid);
        }

        _logger.LogInformation("Checking whether user exists with the given token");
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ConfirmationToken == token);
        if (user is null)
        {
            _logger.LogError("User not found with the given token");
            return Result.Failure(UserErrors.TokenInvalid);
        }

        _logger.LogInformation("Checking whether user already exists with the TempEmail user is trying to confirm");
        var user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.TempEmail);
        if (user1 != null)
        {
            _logger.LogError("User already exists with the TempEmail");
            return Result.Failure(UserErrors.UserAlreadyExists);
        }

        _logger.LogInformation("Updating user's confirmation token and email");
        user.ConfirmationToken = null;
        user.Email = user.TempEmail ?? user.Email;

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Email confirmed successfully");
        return Result.Success();
    }

    public async Task<Result> ChangeEmailAsync(ChangeEmailRequest request, Guid id)
    {
        if (string.IsNullOrEmpty(request.NewEmail) || !IsValidEmail(request.NewEmail))
        {
            _logger.LogError("Invalid email format");
            return Result.Failure(UserErrors.EmailFormatInvalid);
        }

        var user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            _logger.LogError("User not found");
            throw new Exception(UserErrors.UserNotFound.Message);
        }

        _logger.LogInformation("Generating confirmation token for the new email");
        var confirmationToken = GenerateConfirmationToken();

        _logger.LogInformation("Updating user's TempEmail and confirmation token in database");
        user.TempEmail = request.NewEmail;
        user.ConfirmationToken = confirmationToken;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Sending confirmation email to the new email");
        await _emailService.SendEmailConfirmationEmail(request.NewEmail, confirmationToken);

        _logger.LogInformation("Email change request processed successfully");
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, Guid id)
    {
        if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.OldPassword))
        {
            _logger.LogError("New password or old password is empty");
            return Result.Failure(UserErrors.PasswordEmpty);
        }

        _logger.LogInformation("Checking whether user exists in the database");
        var user = await _dbContext.Users.FindAsync(id);

        if (user == null)
        {
            _logger.LogError("User not found");
            throw new Exception(UserErrors.UserNotFound.Message);
        }

        _logger.LogInformation("Checking whether old password is correct");
        var result = VerifyPassword(request.OldPassword, user.Password);
        if (!result)
        {
            _logger.LogError("Incorrect old password");
            return Result.Failure(UserErrors.IncorrectOldPassword);
        }

        _logger.LogInformation("Checking whether new password meets the requirements");
        if (!IsValidPassword(request.NewPassword))
        {
            _logger.LogError("Incorrect new password");
            return Result.Failure(UserErrors.IncorrectNewPassword);
        }

        _logger.LogInformation("Hashing the new password");
        string hashedPassword = HashPassword(request.NewPassword);

        user.Password = hashedPassword;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Password changed successfully");
        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            _logger.LogError("Token is empty");
            return Result.Failure(UserErrors.TokenInvalid);
        }

        if (string.IsNullOrEmpty(request.NewPassword))
        {
            _logger.LogError("New password is empty");
            return Result.Failure(UserErrors.PasswordEmpty);
        }
        else if (!IsValidPassword(request.NewPassword))
        {
            _logger.LogError("New password does not meet the requirements");
            return Result.Failure(UserErrors.PasswordPolicyNotMet);
        }

        if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
        {
            _logger.LogError("Invalid email format");
            return Result.Failure(UserErrors.EmailFormatInvalid);
        }

        _logger.LogInformation("Checking whether user exists with the given email");
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
        {
            _logger.LogError("User not found");
            return Result.Failure(UserErrors.UserNotFound);
        }
        else if (user.ConfirmationToken != request.Token && !user.IsEmailConfirmed)
        {
            _logger.LogError("Email not confirmed and password reset token is invalid");
            return Result.Failure(UserErrors.EmailNotConfirmedAndPasswordReset);
        }
        else if (user.ConfirmationToken != request.Token)
        {
            _logger.LogError("Password reset token is invalid");
            return Result.Failure(UserErrors.TokenInvalid);
        }

        _logger.LogInformation("Hashing the new password");
        user.Password = HashPassword(request.NewPassword);
        user.ConfirmationToken = null;

        _logger.LogInformation("Updating user's password in the database");
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Sending password changed successfully email");
        await _emailService.SendPasswordChangedSuccessfullyEmail(request.Email);

        _logger.LogInformation("Password reset successfully");
        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
        {
            _logger.LogError("Invalid email format");
            return Result.Failure(UserErrors.EmailFormatInvalid);
        }

        _logger.LogInformation("Checking whether user exists with the given email");
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Will return success as we don't want to expose whether the email exists in the database
        if (user is null)
        {
            _logger.LogInformation("User not found");
            return Result.Success();
        }
        if (!user.IsEmailConfirmed)
        {
            _logger.LogError("Email not confirmed");
            return Result.Failure(UserErrors.EmailNotConfirmed);
        }

        _logger.LogInformation("Generating confirmation token for password reset");
        var confirmationToken = GenerateConfirmationToken();

        _logger.LogInformation("Updating user's confirmation token in database");
        user.ConfirmationToken = confirmationToken;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Sending password reset email");
        await _emailService.SendPasswordResetEmail(request.Email, confirmationToken);

        _logger.LogInformation("Password reset request processed successfully");
        return Result.Success();
    }

    #region Private methods

    private async Task<Result<bool>> IsExistingUser(string email)
    {
        _logger.LogInformation("Checking whether user exists with the given email");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            _logger.LogInformation("Not an existing user");
            return Result.Success(false);
        }
        else if (user.IsEmailConfirmed)
        {
            _logger.LogInformation("User exists and is verified");
            return Result.Success(true);
        }
        else
        {
            _logger.LogWarning("User exists but is not verified");
            return Result.Failure<bool>(UserErrors.EmailNotConfirmed);
        }
    }

    private bool IsValidPassword(string password)
    {
        _logger.LogInformation("Validating password");
        try
        {
            string passkey = Encoding.UTF8.GetString(Convert.FromBase64String(password));
            bool isLongEnough = passkey.Length >= _options.MinimumPasswordLength;
            bool hasUppercase = passkey.Any(char.IsUpper);
            bool hasLowercase = passkey.Any(char.IsLower);
            bool hasNumber = passkey.Any(char.IsDigit);
            bool hasSymbol = passkey.Any(c => !char.IsLetterOrDigit(c));

            return isLongEnough && hasUppercase && hasLowercase && hasNumber && hasSymbol;
        }
        catch (Exception)
        {
            _logger.LogError("Password is not base64 encoded");
            return false;
        }
    }

    private string HashPassword(string password)
    {
        byte[] passwordBytes = Convert.FromBase64String(password);
        byte[] salt = Encoding.UTF8.GetBytes(_options.PasswordSalt);

        using var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 10000, HashAlgorithmName.SHA512);
        byte[] hash = new byte[48];
        Array.Copy(pbkdf2.GetBytes(32), 0, hash, 0, 32); // 32 bytes for hash
        Array.Copy(salt, 0, hash, 32, 16); // 16 bytes for the salt

        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string password, string dbPassword)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == dbPassword;
    }

    private static bool IsValidEmail(string email)
    {
        Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return EmailRegex.IsMatch(email);
    }

    private static string GenerateConfirmationToken()
    {
        Random rand = new();
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomString = "";

        for (int i = 0; i < 20; i++)
        {
            int index = rand.Next(characters.Length);
            randomString += characters[index];
        }

        return randomString;
    }

    #endregion
}