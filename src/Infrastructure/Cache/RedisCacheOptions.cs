namespace Infrastructure.Cache;

public class RedisCacheOptions
{
    public const string SectionName = "RedisCache";

    public required string ConnectionString { get; init; }
}
