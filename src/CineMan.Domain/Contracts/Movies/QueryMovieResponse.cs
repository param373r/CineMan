namespace CineMan.Domain.Contracts.Movies;

public record QueryMovieResponse(
    int TotalRecordsFound,
    List<MovieDto> Movies
)
{
    public static QueryMovieResponse FromDomain(int totalRecordsFound, List<MovieDto> movies)
    {
        return new QueryMovieResponse(totalRecordsFound, movies);
    }
}