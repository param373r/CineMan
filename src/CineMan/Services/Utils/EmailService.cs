using System.Net;
using System.Runtime.InteropServices;
using CineMan.Domain.Contracts.Bookings;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CineMan.Services.Utils;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendBookingConfirmationEmail(string recipient, GetBookingResponse booking)
    {
        var from = new EmailAddress("pjsm3701@gmail.com", "CineMan Support");
        var to = new EmailAddress(recipient);
        var subject = "CineMan - Booking Confirmation";
        var plainTextContent = $"Hi,\n\nYou have successfully booked {booking.BookedSeats} tickets for the movie '{booking.MovieId}' on {booking.ShowDate}.\n\nThank you for choosing CineMan!";
        var htmlContent = $"<p>Hi,</p><p>You have successfully booked {booking.BookedSeats} tickets for the movie '{booking.MovieId}' on {booking.ShowDate}.</p><p>Thank you for choosing CineMan!</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        _logger.LogInformation("Sending booking confirmation email to {Recipient}", recipient);
        await SendEmailAsync(msg);
    }

    public async Task SendBookingCancellationEmail(string recipient, GetBookingResponse booking)
    {
        var from = new EmailAddress("pjsm3701@gmail.com", "CineMan Support");
        var to = new EmailAddress(recipient);
        var subject = "CineMan - Booking Cancelled";
        var plainTextContent = $"Hi,\n\nYou have successfully cancelled your bookings for '{booking.MovieId}' on {booking.ShowDate}.\n\nSorry to see you go!";
        var htmlContent = $"<p>Hi,</p><p>You have successfully cancelled your bookings for '{booking.MovieId}' on {booking.ShowDate}.</p><p>Sorry to see you go!</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        _logger.LogInformation("Sending booking cancellation email to {Recipient}", recipient);
        await SendEmailAsync(msg);
    }

    public async Task SendEmailConfirmationEmail(string recipient, string token)
    {
        var from = new EmailAddress("pjsm3701@gmail.com", "CineMan Support");
        var to = new EmailAddress(recipient);
        var subject = "CineMan - Email Confirmation";
        var plainTextContent = $"Hi,\n\nPlease confirm your email by clicking on the following link: {_config["SendGrid:EmailConfirmationUri"]}{token}";
        var htmlContent = $"<p>Hi,</p><p>Please confirm your email by clicking on the following link: {_config["SendGrid:EmailConfirmationUri"]}{token}</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        _logger.LogInformation("Sending email confirmation email to {Recipient}", recipient);
        await SendEmailAsync(msg);
    }

    public async Task SendPasswordResetEmail(string recipient, string token)
    {
        var from = new EmailAddress("pjsm3701@gmail.com", "CineMan Support");
        var to = new EmailAddress(recipient);
        var subject = "CineMan - Password Reset Request";
        var plainTextContent = $"Hi,\n\nHere is your forgot password link: {_config["SendGrid:PasswordResetUri"]}{token}";
        var htmlContent = $"<p>Hi,</p><p>Here is your forgot password link: {_config["SendGrid:PasswordResetUri"]}{token}</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        _logger.LogInformation("Sending password reset email to {Recipient}", recipient);
        await SendEmailAsync(msg);
    }

    public async Task SendPasswordChangedSuccessfullyEmail(string recipient)
    {
        var from = new EmailAddress("pjsm3701@gmail.com", "CineMan Support");
        var to = new EmailAddress(recipient);
        var subject = "CineMan - Password Reset Successfully";
        var plainTextContent = $"Hi,\n\nThis is to inform you that your password has been successfully changed. If this wasn't you please contact support ASAP.";
        var htmlContent = $"<p>Hi,</p><p>This is to inform you that your password has been successfully changed. If this wasn't you please contact support ASAP.</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        _logger.LogInformation("Sending password changed successfully email to {Recipient}", recipient);
        await SendEmailAsync(msg);
    }


    private async Task SendEmailAsync(SendGridMessage message)
    {
        var apiKey = _config["SendGrid:Key"];

        var client = new SendGridClient(apiKey);
        var response = await client.SendEmailAsync(message);

        if (response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Failed to send email. StatusCode={StatusCode}", response.StatusCode);
            throw new Exception($"Failed to send email. StatusCode={response.StatusCode}");
        }
    }
}
