using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CineMan.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // This is where all the unhandled exceptions will be caught
        var response = httpContext.Response;
        response.ContentType = "application/json";

        // TODO: Error and log
        var error = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Instance = httpContext.Request.Path,
            Type = "https://tools.ietf.org/rfc/rfc7231#section-6.6.1"
        };

        await response.WriteAsJsonAsync(error, cancellationToken);
        return true;
    }
}