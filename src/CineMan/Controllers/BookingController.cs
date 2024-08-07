using CineMan.Persistence;
using CineMan.Domain.Contracts.Bookings;
using Microsoft.AspNetCore.Mvc;
using CineMan.Domain.Contracts.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMan.Controllers;

[ApiController]
[Route("/bookings")]
[Produces("application/json")]
[Consumes("application/json")]
[SwaggerTag("Endpoints related to user bookings")]
public class BookingController : ApiController
{
    private readonly IBookingRepository _bookingRepository;

    public BookingController(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new booking",
        Description = "Create a new booking for the user"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Returns 201 when a successful booking is created successfully.", typeof(Response<Guid>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 if the request is invalid.", typeof(ProblemResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Returns 404 if the requested showtime was not found.", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> Create(CreateBookingRequest request)
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await _bookingRepository.CreateBookingAsync(request, id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return CreatedAtAction(
            nameof(GetBookingById),
            new { bookingId = result.GetValue() },
            SuccessResponse(result.GetValue()));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetBookingResponse>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all bookings",
        Description = "Get all bookings for the user"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 with the list of bookings.", typeof(Response<List<GetBookingResponse>>))]
    public async Task<ActionResult<Response>> GetBookings()
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await _bookingRepository.GetBookingsAsync(id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpGet("{bookingId:guid}")]
    [SwaggerOperation(
        Summary = "Get a booking by ID",
        Description = "Get a booking by the provided booking ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 with the booking details.", typeof(Response<GetBookingResponse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Returns 404 if the requested booking was not found.", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> GetBookingById(Guid bookingId)
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await _bookingRepository.GetBookingByIdAsync(bookingId, id);

        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpDelete("{bookingId:guid}")]
    [SwaggerOperation(
        Summary = "Cancel a booking",
        Description = "Cancel a booking by the provided booking ID"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Returns 204 when the booking is canceled successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 if the request is invalid.", typeof(ProblemResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Returns 404 if the requested booking was not found.", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> CancelBooking(Guid bookingId)
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await _bookingRepository.CancelBookingAsync(bookingId, id);

        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return NoContent();
    }
}