namespace CineMan.Errors.ErrorConstants;

public static class BookingErrors
{
    // TODO: Add more booking errors
    public static Error BookingNotFound = new("booking.id.notfound", "Make sure you're entering the right booking id", 404);
    public static Error CancellingPastTickets = new("booking.cancelling.dateinpast", "You cannot cancel the tickets after the show has taken place.", 400);
    public static Error ShowAlreadyCancelled = new("booking.show.alreadycancelled", "You cannot cancel the booking again", 400);
    public static Error ShowNotAvailable = new("booking.show.unavailable", "This showtime is not available. Please refresh to get the updated shows", 404);
    public static Error TimeSlotNotAvailable = new("booking.timeslot.unavailable", "This show doesn't have the chosen timeslot available, kindly pick another.", 404);
    public static Error SeatsNotAvailable = new("booking.timeslot.seatsunavailable", "Requested number of seats not available in this timeslot. Try another.", 404);
    public static Error ShowDateIsInPast = new("booking.showdate.isinpast", "You cannot book tickets of past shows", 400);
}