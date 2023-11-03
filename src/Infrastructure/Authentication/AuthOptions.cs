namespace Ok.Movies.Infrastructure.Authentication;

public class AuthOptions
{
    public const string SectionName = "Auth";

    public required string ApiKey { get; init; }
}
