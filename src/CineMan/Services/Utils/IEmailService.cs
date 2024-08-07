using CineMan.Domain.Contracts.Bookings;

namespace CineMan.Services.Utils;

public interface IEmailService
{
    Task SendBookingConfirmationEmail(string email, GetBookingResponse booking);
    Task SendBookingCancellationEmail(string email, GetBookingResponse booking);
    Task SendEmailConfirmationEmail(string email, string token);
    Task SendPasswordResetEmail(string email, string token);
    Task SendPasswordChangedSuccessfullyEmail(string email);
}
