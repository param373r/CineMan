using CineMan.Domain.Contracts.Response;
using CineMan.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CineMan.Controllers;

public abstract class ApiController : ControllerBase
{
    protected ProblemResponse FailureResponse(Error error)
    {
        var problem = new ProblemResponse(
            Type: $"https://httpstatuses.com/{error.StatusCode}",
            Status: error.StatusCode,
            Title: error.Message,
            Detail: error.Details,
            Instance: Request.Path,
            CorrelationId: HttpContext.Request.Headers["X-Correlation-ID"].ToString()
        );
        HttpContext.Response.ContentType = "application/problem+json";
        return problem;
    }

    protected Response SuccessResponse<T>(T value)
    {
        return new Response<T>(value, HttpContext.Request.Headers["X-Correlation-ID"].ToString());
    }

    protected Response SuccessResponse()
    {
        return new Response(HttpContext.Request.Headers["X-Correlation-ID"].ToString());
    }
}