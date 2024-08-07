using CineMan.Domain.Models.AvailableShowTimes;

namespace CineMan.Domain.Contracts.Movies;

public record AvailableShowTimesDto(
    Guid MovieId,
    DateOnly ShowDate,
    string TheatreName,
    Dictionary<TimeSlot, int> SeatPerTimeSlot,
    decimal PricePerSeat
);
