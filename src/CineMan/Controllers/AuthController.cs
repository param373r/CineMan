using CineMan.Domain.Contracts.Auth;
using CineMan.Domain.Contracts.Response;
using CineMan.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMan.Controllers;

[ApiController]
[Route("/auth")]
[Produces("application/json")]
[Consumes("application/json")]
[SwaggerTag("Endpoints related to identity management")]
public class AuthController : ApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Register a new user with the provided credentials"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Returns 201 when a user is successfully registered", typeof(Response))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 for an Invalid Request", typeof(ProblemResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Returns 403 if a user already exists with the provided email", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return CreatedAtAction(
            nameof(LoginUser),
            SuccessResponse()
        );
    }

    [HttpPost("/login")]
    [SwaggerOperation(
        Summary = "Login a user",
        Description = "Login a user with the provided credentials"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 when a login is successful", typeof(Response<TokenResponse>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns 401 when a credentials are invalid", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> LoginUser([FromBody] LoginUserRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpPost("/refresh")]
    [SwaggerOperation(
        Summary = "Refresh access token",
        Description = "Refresh access token with the provided refresh token")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 when access token is successfully refreshed", typeof(Response<TokenResponse>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns 401 when the refresh token is invalid or expired", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> GetAccessToken([FromBody] AccessTokenRequest request)
    {
        var result = await _authService.RefreshAccessTokenAsync(request);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpPost("/changePassword")]
    [SwaggerOperation(
        Summary = "Change user password",
        Description = "Change user password with the provided credentials")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 when the password is successfully changed", typeof(Response))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 for an Invalid Request", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await _authService.ChangePasswordAsync(request, id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse());
    }

    [HttpPost("/changeEmail")]
    [SwaggerOperation(
        Summary = "Change user email",
        Description = "Change user email with the provided credentials")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 when the email is successfully changed", typeof(Response))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 for an Invalid Request", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> ChangeEmail([FromBody] ChangeEmailRequest request)
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await _authService.ChangeEmailAsync(request, id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse());
    }

    [HttpPost("/forgotPassword")]
    [SwaggerOperation(
        Summary = "Forgot password",
        Description = "Forgot password with the provided email")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 when the password reset email is successfully sent", typeof(Response))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 for an Invalid Request", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }
        return Ok(SuccessResponse());
    }

    [HttpPost("/resetPassword")]
    [SwaggerOperation(
        Summary = "Reset password",
        Description = "Reset password with the provided token and new password")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Returns 204 when the password is successfully reset")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 for an Invalid Request", typeof(ProblemResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Returns 403 if the reset token is invalid or expired", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return NoContent();
    }
}