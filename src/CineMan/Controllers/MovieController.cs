using Microsoft.AspNetCore.Mvc;
using CineMan.Domain.Contracts.Movies;
using CineMan.Persistence;
using CineMan.Domain.Contracts.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMan.Controllers;

[ApiController]
[Route("/movies")]
[Produces("application/json")]
[Consumes("application/json")]
[SwaggerTag("Endpoints related to movie querying and search")]
public class MovieController : ApiController
{
    private readonly IMovieRepository _movieRepository;

    public MovieController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpGet("query")]
    [SwaggerOperation(
        Summary = "Get query parameters",
        Description = "Get the query parameters for searching movies"
    )]
    [SwaggerResponse(200, "Returns 200 when query parameters are fetched", typeof(Response<GetQueryParametersResponse>))]
    [SwaggerResponse(401, "Returns 401 if the user making this request is not authorized", typeof(ProblemResponse))]
    public ActionResult<Response> GetQueryParameters()
    {
        var result = _movieRepository.GetQueryParameters();

        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpPost("query")]
    [SwaggerOperation(
        Summary = "Search movies",
        Description = "Search movies based on the provided query parameters"
    )]
    [SwaggerResponse(200, "Returns 200 when movies are successfully searched", typeof(Response<QueryMovieResponse>))]
    [SwaggerResponse(401, "Returns 401 if the user making this request is not authorized", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> SearchMovies([FromBody] QueryMovieRequest request, [FromQuery] int pageNumber, [FromQuery] int resultPerPage)
    {
        var result = await _movieRepository.GetMoviesAsync(request, pageNumber, resultPerPage);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }

        return Ok(SuccessResponse(result.GetValue()));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get movie by ID",
        Description = "Get a movie by its ID"
    )]
    [SwaggerResponse(200, "Returns 200 when the movie is found", typeof(Response<GetMovieResponse>))]
    [SwaggerResponse(401, "Returns 401 if the user making this request is not authorized", typeof(ProblemResponse))]
    [SwaggerResponse(404, "Returns 404 if the movie is not found", typeof(ProblemResponse))]
    public async Task<ActionResult<Response>> GetMovieById(Guid id)
    {
        var result = await _movieRepository.GetMovieByIdAsync(id);
        if (result.IsFailure)
        {
            var response = FailureResponse(result.Error!);
            return StatusCode(response.Status, response);
        }
        return Ok(SuccessResponse(result.GetValue()));
    }
}
