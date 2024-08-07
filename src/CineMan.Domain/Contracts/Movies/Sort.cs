namespace CineMan.Domain.Contracts.Movies;

public class Sort
{
    public SortOrder SortOrder { get; set; }
    public SortBy SortBy { get; set; }
}

public enum SortBy
{
    TITLE,
    RATING,
    RELEASE_DATE,
    DURATION,
}

public enum SortOrder
{
    ASCENDING,
    DESCENDING
}
