using Application.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IMovieRepository, MovieRepository>();

        return services;
    }
}
