using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.MinimalAPI.Infrastructure.Cache;
using Ok.Movies.MinimalAPI.Infrastructure.Database;
using Ok.Movies.MinimalAPI.Infrastructure.Health;
using Ok.Movies.MinimalAPI.Infrastructure.Middleware;
using Ok.Movies.MinimalAPI.Infrastructure.OpenApi;
using Ok.Movies.MinimalAPI.Infrastructure.Versioning;

namespace Ok.Movies.MinimalAPI.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services
            .AddExceptionMiddleware(environment)
            .AddDatabase()
            .AddJwtAuthentication()
            .AddAppAuthorization()
            .AddVersioning()
            .AddOpenApiDocumentation()
            .AddHealthCheck()
            // .AddResponseCaching()
            .AddOutputCaching()
            ;

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.MapHealthCheck();
        app.UseOpenApiDocumentation();
        app.UseAuthentication()
            .UseAuthorization();
        // .UseCors()
        // .UseResponseCaching()
        app.UseOutputCaching();
        app.UseExceptionMiddleware();

        return app;
    }
}
