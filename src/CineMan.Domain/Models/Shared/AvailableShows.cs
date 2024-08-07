using CineMan.Domain.Models.Abstractions;
using CineMan.Domain.Models.Movies;

namespace CineMan.Domain.Models.Shared;

public abstract class AvailableShows : Entity
{
    public Guid MovieId { get; set; }
    public Movie? Movie { get; set; }
    public DateOnly ShowDate { get; set; }
    public required string TheatreName { get; set; }

    public AvailableShows()
    {
    }
}