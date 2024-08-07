using CineMan.Domain.Models.AvailableShowTimes;
using CineMan.Domain.Models.Shared;
using CineMan.Domain.Models.Users;

namespace CineMan.Domain.Models.UserBookings;

public class UserBooking : AvailableShows
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public int BookedSeats { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public DateTime OrderDate { get; set; }

    public UserBooking() : base()
    {
    }
}