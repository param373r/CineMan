using CineMan.Domain.Models.Abstractions;
using CineMan.Domain.Models.UserBookings;

namespace CineMan.Domain.Models.Users;

public class User : Entity
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Address? Address { get; set; }
    public required string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? TempEmail { get; set; }
    public string? ConfirmationToken { get; set; }
    public required string Password { get; set; }
    public List<Guid>? Bookings { get; set; }
    public List<UserBooking>? UserBookings { get; set; }

    public User()
    {
    }
}
