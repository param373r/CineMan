using CineMan.Domain.Contracts.Response;
using CineMan.Options;
using CineMan.Services.Utils;
using Microsoft.Extensions.Options;

namespace CineMan.Middlewares;

public class AuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IJwtService jwtService, IOptions<AuthOptions> options)
    {
        // Checking whether path is excluded
        if (options.Value.ExcludePaths.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);

        // Validating the token
        var principal = jwtService.ValidateAccessToken(token);
        if (principal == null)
        {
            // TODO: Error and log; Create a problem response
            var response = new ProblemResponse(
                Type: "https://httpstatuses.com/401",
                Status: 401,
                Title: "Unauthorized",
                Detail: "Invalid token",
                Instance: context.Request.Path,
                null
            );

            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        // Checking whether auth is allowed;
        if (principal.Claims.First(c => c.Type == "allowLogin").Value == "false")
        {
            // TODO: Error and log
            var response = new ProblemResponse(
                Type: "https://httpstatuses.com/403",
                Status: 403,
                Title: "Forbidden",
                Detail: "User is not allowed to make requests. Please reach out to support.",
                Instance: context.Request.Path,
                null
            );

            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        context.User = principal;
        await next(context);
    }
}