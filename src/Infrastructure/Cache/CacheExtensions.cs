using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ok.Movies.MinimalAPI.Contracts.Requests;
using StackExchange.Redis;

namespace Ok.Movies.MinimalAPI.Infrastructure.Cache;

public static class CacheExtensions
{
    public const string MovieCachePolicy = "MovieCache";
    public const string MoviesTag = "movies";
    
    public static IServiceCollection AddOutputCaching(this IServiceCollection services)
    {
        return services
            .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(
                ConfigurationOptions.Parse("localhost,abortConnect=false,connectTimeout=30000,responseTimeout=30000")))
            .AddRedisOutputCache(options=>
            {
                options.AddBasePolicy(builder => builder.Cache());
                options.AddPolicy(MovieCachePolicy, builder =>
                {
                    builder.Cache()
                        .Expire(TimeSpan.FromSeconds(10))
                        .SetVaryByQuery(new[]
                        {
                            nameof(GetAllMoviesRequest.Title), nameof(GetAllMoviesRequest.SortBy),
                            nameof(GetAllMoviesRequest.Page), nameof(GetAllMoviesRequest.PageSize)
                        })
                        .Tag(MoviesTag);
                });
            });
    }

    public static IApplicationBuilder UseOutputCaching(this IApplicationBuilder app)
    {
        return app.UseOutputCache();
    }
}
