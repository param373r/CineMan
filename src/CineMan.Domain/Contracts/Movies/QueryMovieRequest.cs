using CineMan.Domain.Models.Movies;

namespace CineMan.Domain.Contracts.Movies;

public record QueryMovieRequest
{
    public string? Title { get; set; }
    public Genre? Genre { get; set; }
    public Language? Language { get; set; }
    public Format? Format { get; set; }
    public Sort? Sort { get; set; }
}