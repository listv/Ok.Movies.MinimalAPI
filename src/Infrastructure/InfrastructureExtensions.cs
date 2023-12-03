using Infrastructure.Authentication;
using Infrastructure.Cache;
using Infrastructure.Database;
using Infrastructure.Health;
using Infrastructure.Middleware;
using Infrastructure.OpenApi;
using Infrastructure.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

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
