namespace CineMan.Domain.Contracts.Movies;

public record MovieDto(
    Guid MovieId,
    string Title,
    string Description,
    string Rating,
    string? Poster
);