using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ok.Movies.Application.Repositories;
using Ok.Movies.Application.Services;

namespace Ok.Movies.Application;

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
