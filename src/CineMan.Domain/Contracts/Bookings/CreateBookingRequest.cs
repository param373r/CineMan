using CineMan.Domain.Models.AvailableShowTimes;

namespace CineMan.Domain.Contracts.Bookings;

public record CreateBookingRequest(
    Guid ShowTimeId,
    int TotalRequestedSeats,
    TimeSlot TimeSlot
);
