using CineMan.Domain.Contracts.Bookings;
using CineMan.Errors;

namespace CineMan.Persistence;

public interface IBookingRepository
{
    Task<Result<Guid>> CreateBookingAsync(CreateBookingRequest booking, Guid userId);
    Task<Result<GetBookingResponse>> GetBookingByIdAsync(Guid id, Guid userId);
    Task<Result<List<GetBookingResponse>>> GetBookingsAsync(Guid userId);
    Task<Result> CancelBookingAsync(Guid id, Guid userId);
}