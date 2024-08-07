using CineMan.Domain.Models.AvailableShowTimes;
using CineMan.Domain.Models.UserBookings;

namespace CineMan.Domain.Contracts.Bookings;

public record GetBookingResponse(
    Guid Id,
    int BookedSeats,
    Guid MovieId,
    DateOnly ShowDate,
    string TheatreName,
    TimeSlot TimeSlot,
    BookingStatus Status,
    decimal TotalAmount,
    DateTime OrderDate
)
{
    public static GetBookingResponse FromDomain(UserBooking booking)
    {
        return new GetBookingResponse(
            booking.Id,
            booking.BookedSeats,
            booking.MovieId,
            booking.ShowDate,
            booking.TheatreName,
            booking.TimeSlot,
            booking.Status,
            booking.TotalAmount,
            booking.OrderDate
        );
    }
}