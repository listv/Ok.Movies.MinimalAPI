using Application.Database;
using Application.Repositories;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        services.AddProblemDetails(ConfigureProblemDetails(environment));

        return services;
    }

    private static Action<ProblemDetailsOptions> ConfigureProblemDetails(IHostEnvironment environment)
    {
        return options =>
        {
            options.IncludeExceptionDetails = (_, _) => environment.IsDevelopment();

            options.MapFluentValidationException();
        };
    }

    

    public static IServiceCollection AddDatabase(this IServiceCollection services, string? connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        return services;
    }
}
