using Application.Repositories;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddSingleton<IMovieRepository, MovieRepository>()
            .AddSingleton<IMovieService, MovieService>()
            .AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
    }
}
