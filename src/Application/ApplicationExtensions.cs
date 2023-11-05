using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ok.Movies.MinimalAPI.Application.Repositories;
using Ok.Movies.MinimalAPI.Application.Services;

namespace Ok.Movies.MinimalAPI.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddSingleton<IRatingRepository, RatingRepository>()
            .AddSingleton<IRatingService, RatingService>()
            .AddSingleton<IMovieRepository, MovieRepository>()
            .AddSingleton<IMovieService, MovieService>()
            .AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
    }
}
