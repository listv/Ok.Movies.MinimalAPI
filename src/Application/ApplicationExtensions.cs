using Application.Database;
using Application.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.AddDatabase(config.GetConnectionString("movies"));
        services.AddSingleton<IMovieRepository, MovieRepository>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, string? connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        return services;
    }
}
