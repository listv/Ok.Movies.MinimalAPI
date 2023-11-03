using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ok.Movies.Contracts.Requests;

namespace Ok.Movies.Infrastructure.Cache;

public static class CacheExtensions
{
    public const string MovieCachePolicy = "MovieCache";
    public const string MoviesTag = "movies";
    
    public static IServiceCollection AddOutputCaching(this IServiceCollection services)
    {
        return services
            .AddOutputCache(options =>
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
