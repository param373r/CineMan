using CineMan.Domain.Contracts.Movies;
using CineMan.Domain.Models.Movies;
using CineMan.Errors;
using CineMan.Errors.ErrorConstants;
using CineMan.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CineMan.Persistence;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<MovieRepository> _logger;

    public MovieRepository(AppDbContext dbContext, ILogger<MovieRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<GetMovieResponse>> GetMovieByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting movie by ID: {MovieId}", id);

        var movie = await _dbContext.Movies.Where(m => m.Id == id).FirstOrDefaultAsync();
        if (movie is null)
        {
            _logger.LogError("Movie with ID {MovieId} not found", id);
            return Result.Failure<GetMovieResponse>(MovieErrors.MovieWithIdNotFound);
        }

        _logger.LogInformation("Retrieving show times for movie with ID: {MovieId}", id);
        var showTimes = await _dbContext.AvailableShowTimes.Where(ast => ast.MovieId == movie.Id).ToListAsync();

        _logger.LogInformation("Movie with ID {MovieId} retrieved successfully", id);
        return Result.Success(GetMovieResponse.FromDomain(movie, showTimes));
    }

    public async Task<Result<QueryMovieResponse>> GetMoviesAsync(QueryMovieRequest request, int pageNumber, int resultPerPage)
    {
        _logger.LogInformation("Getting movies with query: {Query}", request);

        var query = _dbContext.Movies.AsQueryable();

        // Filtering by language if provided
        if (request.Language != null)
        {
            _logger.LogInformation("Filtering movies by language: {Language}", request.Language);
            query = query.Where(m => m.Language == request.Language);
        }

        // Filtering by format if provided
        if (request.Format != null)
        {
            _logger.LogInformation("Filtering movies by format: {Format}", request.Format);
            query = query.Where(m => m.Format == request.Format);
        }

        // Filtering by genre if provided
        if (request.Genre != null)
        {
            _logger.LogInformation("Filtering movies by genre: {Genre}", request.Genre);
            query = query.Where(m => m.Genre == request.Genre);
        }

        // Filtering by title if provided
        if (!string.IsNullOrEmpty(request.Title))
        {
            _logger.LogInformation("Filtering movies by title: {Title}", request.Title);
            query = query.Where(m => m.Name.Contains(request.Title));
        }

        // Sorting the query
        if (request.Sort == null)
        {
            _logger.LogInformation("Sorting movies by default (name)");
            query = query.OrderBy(m => m.Name);
        }
        else
        {
            _logger.LogInformation("Sorting movies by {SortBy} in {SortOrder} order", request.Sort.SortBy, request.Sort.SortOrder);
            query = request.Sort.SortBy switch
            {
                SortBy.TITLE => (request.Sort.SortOrder == SortOrder.ASCENDING)
                    ? query.OrderBy(m => m.Name)
                    : query.OrderByDescending(m => m.Name),
                SortBy.RATING => (request.Sort.SortOrder == SortOrder.ASCENDING)
                    ? query.OrderBy(m => m.Rating)
                    : query.OrderByDescending(m => m.Rating),
                SortBy.RELEASE_DATE => (request.Sort.SortOrder == SortOrder.ASCENDING)
                    ? query.OrderBy(m => m.ReleaseDate)
                    : query.OrderByDescending(m => m.ReleaseDate),
                SortBy.DURATION => (request.Sort.SortOrder == SortOrder.ASCENDING)
                    ? query.OrderBy(m => m.RunningTime)
                    : query.OrderByDescending(m => m.RunningTime),
                _ => query
            };
        }

        var count = await query.CountAsync();

        // Implementing pagination
        query = query
            .Skip((pageNumber - 1) * resultPerPage)
            .Take(resultPerPage);

        // Querying the database
        var movies = await query.Select(m => new MovieDto(
            m.Id,
            m.Name,
            m.Description,
            m.Rating,
            m.PosterUrl
        )).ToListAsync();

        _logger.LogInformation("Retrieved {Count} movies", movies.Count);

        return Result.Success(QueryMovieResponse.FromDomain(count, movies));
    }

    public Result<GetQueryParametersResponse> GetQueryParameters()
    {
        _logger.LogInformation("Reading query parameters");
        var languages = Enum.GetValues<Language>();
        var formats = Enum.GetValues<Format>();
        var genres = Enum.GetValues<Genre>();
        var sortBy = Enum.GetValues<SortBy>();

        return Result.Success(GetQueryParametersResponse.FromDomain(genres, formats, languages, sortBy));
    }
}
