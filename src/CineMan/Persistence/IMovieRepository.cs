using CineMan.Domain.Contracts.Movies;
using CineMan.Errors;

namespace CineMan.Persistence;

public interface IMovieRepository
{
    Task<Result<QueryMovieResponse>> GetMoviesAsync(QueryMovieRequest request, int pageNumber, int resultPerPage);
    Task<Result<GetMovieResponse>> GetMovieByIdAsync(Guid id);
    Result<GetQueryParametersResponse> GetQueryParameters();
}