namespace Contracts.Requests;

public class MoviesFilteringRequest
{
    public required string? Title { get; init; }
    public required int? Year { get; init; }
}
