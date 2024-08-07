namespace CineMan.Domain.Contracts.Response;

public record Response
{
    public string? CorrelationId { get; init; }

    public Response(string? correlationId)
    {
        CorrelationId = string.IsNullOrEmpty(correlationId) ? Guid.NewGuid().ToString() : correlationId;
    }
}

public record Response<T>(
    T? Result,
    string? CorrelationId
) : Response(CorrelationId);