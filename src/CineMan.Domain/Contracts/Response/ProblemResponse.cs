namespace CineMan.Domain.Contracts.Response;

public record ProblemResponse(
    string Type,
    int Status,
    string Title,
    string Detail,
    string Instance,
    string? CorrelationId
) : Response(CorrelationId);