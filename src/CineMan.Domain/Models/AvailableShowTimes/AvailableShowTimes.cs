using CineMan.Domain.Models.Shared;

namespace CineMan.Domain.Models.AvailableShowTimes;

public class AvailableShowTimes : AvailableShows
{
    public Dictionary<TimeSlot, int> SeatPerTimeSlot { get; set; }
    public int PricePerSeat { get; set; }

    public AvailableShowTimes() : base()
    {
        SeatPerTimeSlot = [];
    }
}