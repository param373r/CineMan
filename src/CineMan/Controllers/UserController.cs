using CineMan.Persistence;
using CineMan.Domain.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using CineMan.Domain.Contracts.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMan.Controllers;

[ApiController]
[Route("/users")]
[Produces("application/json")]
[Consumes("application/json")]
[SwaggerTag("Endpoints related to user profile management")]
public class UserController : ApiController
{
    private readonly IUserRepository userRepository;

    public UserController(IUserRepository _userRepository)
    {
        userRepository = _userRepository;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get user profile",
        Description = "Get the user profile"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 after fetching the user profile", typeof(Response<GetUserResponse>))]
    public async Task<ActionResult<Response>> GetProfile()
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await userRepository.GetUserProfileAsync(id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(result.Error!.StatusCode, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update user profile",
        Description = "Update the user profile"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 after successfully updating the user profile in order to track correlationId", typeof(Response))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns 400 if the request is invalid", typeof(ProblemResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Returns 403 if the request is trying to update DOB less than 13 years of age", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> UpdateProfile(UpdateProfileRequest request)
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);

        var result = await userRepository.UpdateProfileAsync(request, id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(result.Error!.StatusCode, response);
        }

        return Ok(SuccessResponse());
    }

    [HttpDelete]
    [SwaggerOperation(
        Summary = "Delete user",
        Description = "Delete the user"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 after successfully deleting the user in order to track the correlationId", typeof(Response))]
    public async Task<ActionResult<Response>> DeleteUser()
    {
        var id = Guid.Parse(HttpContext.User.Identity!.Name!);
        var result = await userRepository.DeleteUserAsync(id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(result.Error!.StatusCode, response);
        }

        return Ok(SuccessResponse());
    }
}