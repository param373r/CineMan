using CineMan.Domain.Models.Movies;

namespace CineMan.Domain.Contracts.Movies;

public record GetQueryParametersResponse(
    Genre[] Genres,
    Format[] Format,
    Language[] Languages,
    SortBy[] SortBy
)
{
    public static GetQueryParametersResponse FromDomain(Genre[] genres, Format[] format, Language[] languages, SortBy[] sortBy)
    {
        return new GetQueryParametersResponse(genres, format, languages, sortBy);
    }
}