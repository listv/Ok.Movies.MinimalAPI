namespace Ok.Movies.MinimalAPI.Infrastructure.Authentication;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
}
