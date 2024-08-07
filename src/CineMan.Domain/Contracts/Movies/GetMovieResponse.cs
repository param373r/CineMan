using CineMan.Domain.Models.AvailableShowTimes;
using CineMan.Domain.Models.Movies;

namespace CineMan.Domain.Contracts.Movies;

public record GetMovieResponse(
    MovieDto Movie,
    List<AvailableShowTimesDto> ShowTimes
)
{
    public static GetMovieResponse FromDomain(Movie movie, List<AvailableShowTimes> availableShowTimes)
    {
        var movieDto = new MovieDto(
            movie.Id,
            movie.Name,
            movie.Description,
            movie.Rating,
            movie.PosterUrl
        );

        var showTimes = new List<AvailableShowTimesDto>();
        foreach (var ast in availableShowTimes)
        {
            showTimes.Add(new AvailableShowTimesDto(
                ast.MovieId,
                ast.ShowDate,
                ast.TheatreName,
                ast.SeatPerTimeSlot,
                ast.PricePerSeat
            ));
        }

        return new GetMovieResponse(movieDto, showTimes);
    }
}