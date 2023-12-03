using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ok.Movies.MinimalAPI.Contracts.Requests;
using Ok.Movies.MinimalAPI.Infrastructure.Validation;
using StackExchange.Redis;

namespace Ok.Movies.MinimalAPI.Infrastructure.Cache;

public static class CacheExtensions
{
    public const string MovieCachePolicy = "MovieCache";
    public const string MoviesTag = "movies";
    
    public static IServiceCollection AddOutputCaching(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<RedisCacheOptions>, RedisCacheOptionsValidator>();

        services.AddOptions<RedisCacheOptions>()
            .BindConfiguration(RedisCacheOptions.SectionName)
            .ValidateFluently()
            .ValidateOnStart();

        var serviceProvider = services.BuildServiceProvider();
        var serviceProviderFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceProviderFactory.CreateScope();
        var redisCacheSettings = serviceScope.ServiceProvider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
        
        return services
            .AddSingleton<IConnectionMultiplexer>(_ => 
                ConnectionMultiplexer.Connect(redisCacheSettings.ConnectionString))
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
