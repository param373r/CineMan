namespace CineMan.Errors;

public record Error(
    string Message,
    string Details,
    int StatusCode
);