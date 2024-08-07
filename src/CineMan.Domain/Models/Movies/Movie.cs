using CineMan.Domain.Models.Abstractions;

namespace CineMan.Domain.Models.Movies;

public sealed class Movie : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
    // TODO: Fetch it from IMDB
    public string Rating { get; set; }
    public string? PosterUrl { get; set; }
    public int RunningTime { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public Genre Genre { get; set; }
    public Format Format { get; set; }
    public Language Language { get; set; }
    public bool IsFeatured { get; set; }

    public Movie()
    {
        Name = string.Empty;
        Description = string.Empty;
        Rating = string.Empty;
    }
}