namespace CineMan.Errors.ErrorConstants;

public class MovieErrors
{
    public static Error MovieWithIdNotFound => new Error("movie.id.notfound", "Please check if specified movie id is correct.", 404);
}