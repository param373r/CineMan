using CineMan.Domain.Contracts.Bookings;
using CineMan.Domain.Models.UserBookings;
using CineMan.Errors;
using CineMan.Errors.ErrorConstants;
using CineMan.Persistence.Data;
using CineMan.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace CineMan.Persistence;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(AppDbContext dbContext, IEmailService emailService, ILogger<BookingRepository> logger)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result> CancelBookingAsync(Guid id, Guid userId)
    {
        var booking = await _dbContext.UserBookings.Where(b => b.UserId == userId && b.Id == id).FirstOrDefaultAsync();
        if (booking == null)
        {
            _logger.LogError($"Booking not found for Id: {id} and UserId: {userId}");
            return Result.Failure(BookingErrors.BookingNotFound);
        }

        // Checking whether the show date is not in the past 
        if (booking.ShowDate < DateOnly.FromDateTime(DateTime.Now))
        {
            _logger.LogError($"Cannot cancel past tickets for Booking Id: {id} and UserId: {userId}");
            return Result.Failure(BookingErrors.CancellingPastTickets);
        }

        // Checking whether the booking is not already cancelled
        if (booking.Status == BookingStatus.CANCELLED)
        {
            _logger.LogError($"Booking with Id: {id} and UserId: {userId} is already cancelled");
            return Result.Failure(BookingErrors.ShowAlreadyCancelled);
        }

        // Marking the booking as cancelled
        booking.Status = BookingStatus.CANCELLED;

        // Updating the available seats for the show
        _logger.LogInformation($"Updating available seats for the show with Id: {booking.MovieId} on {booking.ShowDate} at {booking.TheatreName}");
        var show = await _dbContext.AvailableShowTimes
            .Where(ast => ast.MovieId == booking.MovieId
            && ast.ShowDate == booking.ShowDate
            && ast.TheatreName == booking.TheatreName)
            .FirstOrDefaultAsync();

        show!.SeatPerTimeSlot[booking.TimeSlot] += booking.BookedSeats;

        _logger.LogInformation($"Updating records in the database");
        _dbContext.UserBookings.Update(booking);
        _dbContext.AvailableShowTimes.Update(show);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Sending booking cancellation email to {userId}");
        await _emailService.SendBookingCancellationEmail(booking.UserId.ToString(), GetBookingResponse.FromDomain(booking));

        _logger.LogInformation($"Booking with Id: {id} and UserId: {userId} has been cancelled");
        return Result.Success();
    }

    public async Task<Result<Guid>> CreateBookingAsync(CreateBookingRequest booking, Guid userId)
    {
        _logger.LogInformation($"Checking the availability of the show with ShowTimeId: {booking.ShowTimeId}");
        var show = await _dbContext.AvailableShowTimes.FindAsync(booking.ShowTimeId);
        if (show == null)
        {
            _logger.LogError($"Show not available for ShowTimeId: {booking.ShowTimeId}");
            return Result.Failure<Guid>(BookingErrors.ShowNotAvailable);
        }

        _logger.LogInformation($"Checking the availability of the time slot for ShowTimeId: {booking.ShowTimeId} and TimeSlot: {booking.TimeSlot}");
        var IsTimeSlotAvailable = show.SeatPerTimeSlot.ContainsKey(booking.TimeSlot);
        if (!IsTimeSlotAvailable)
        {
            _logger.LogError($"Time slot not available for ShowTimeId: {booking.ShowTimeId} and TimeSlot: {booking.TimeSlot}");
            return Result.Failure<Guid>(BookingErrors.TimeSlotNotAvailable);
        }
        else
        {
            _logger.LogInformation($"Checking the availability of seats for ShowTimeId: {booking.ShowTimeId}, TimeSlot: {booking.TimeSlot} and TotalRequestedSeats: {booking.TotalRequestedSeats}");
            if (show.SeatPerTimeSlot[booking.TimeSlot] < booking.TotalRequestedSeats)
            {
                _logger.LogError($"Seats not available for ShowTimeId: {booking.ShowTimeId}, TimeSlot: {booking.TimeSlot} and TotalRequestedSeats: {booking.TotalRequestedSeats}");
                return Result.Failure<Guid>(BookingErrors.SeatsNotAvailable);
            }
            else if (show.SeatPerTimeSlot[booking.TimeSlot] >= booking.TotalRequestedSeats)
            {
                _logger.LogInformation($"Updating available seats for ShowTimeId: {booking.ShowTimeId}, TimeSlot: {booking.TimeSlot} and TotalRequestedSeats: {booking.TotalRequestedSeats}");
                show.SeatPerTimeSlot[booking.TimeSlot] -= booking.TotalRequestedSeats;
            }
        }

        _logger.LogInformation($"Checking whether the show date is not in the past for ShowTimeId: {booking.ShowTimeId}");
        if (show.ShowDate <= DateOnly.FromDateTime(DateTime.Now))
        {
            _logger.LogError($"Show date is in the past for ShowTimeId: {booking.ShowTimeId}");
            return Result.Failure<Guid>(BookingErrors.ShowDateIsInPast);
        }

        // Creating a new booking
        var _booking = new UserBooking
        {
            Id = Guid.NewGuid(),
            MovieId = show.MovieId,
            ShowDate = show.ShowDate,
            TheatreName = show.TheatreName,
            UserId = userId,
            BookedSeats = booking.TotalRequestedSeats,
            TotalAmount = booking.TotalRequestedSeats * show.PricePerSeat,
            Status = BookingStatus.BOOKED,
            TimeSlot = booking.TimeSlot,
            OrderDate = DateTime.Now
        };

        _logger.LogInformation($"Updating records in the database");
        _dbContext.UserBookings.Add(_booking);
        _dbContext.AvailableShowTimes.Update(show);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Sending booking confirmation email to {userId}");
        await _emailService.SendBookingConfirmationEmail(userId.ToString(), GetBookingResponse.FromDomain(_booking));

        _logger.LogInformation($"Booking created with Id: {_booking.Id} for ShowTimeId: {booking.ShowTimeId} and UserId: {userId}");
        return Result.Success(_booking.Id);
    }

    public async Task<Result<GetBookingResponse>> GetBookingByIdAsync(Guid id, Guid userId)
    {
        _logger.LogInformation($"Getting booking with Id: {id} for UserId: {userId}");

        var booking = await _dbContext.UserBookings.Where(b => b.UserId == userId && b.Id == id).FirstOrDefaultAsync();

        if (booking == null)
        {
            _logger.LogError($"Booking not found for Id: {id} and UserId: {userId}");
            return Result.Failure<GetBookingResponse>(BookingErrors.BookingNotFound);
        }

        _logger.LogInformation($"Booking found for Id: {id} and UserId: {userId}");
        return Result.Success(GetBookingResponse.FromDomain(booking));
    }

    public async Task<Result<List<GetBookingResponse>>> GetBookingsAsync(Guid userId)
    {
        _logger.LogInformation($"Getting all bookings for UserId: {userId}");
        var bookings = await _dbContext.UserBookings.Where(b => b.UserId == userId).ToListAsync();

        if (bookings.Count == 0)
        {
            _logger.LogWarning($"No bookings found for UserId: {userId}");
        }

        return Result.Success(bookings.Select(GetBookingResponse.FromDomain).ToList());
    }
}