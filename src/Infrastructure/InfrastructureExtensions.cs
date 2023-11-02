using Infrastructure.Authentication;
using Infrastructure.Cache;
using Infrastructure.Database;
using Infrastructure.Health;
using Infrastructure.Middleware;
using Infrastructure.OpenApi;
using Infrastructure.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
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

    public static IApplicationBuilder? UseInfrastructure(this IApplicationBuilder builder)
    {
        return builder
            .UseAuthentication()
            .UseAuthorization()
            // .UseCors()
            // .UseResponseCaching()
            .UseOutputCaching()
            .UseExceptionMiddleware()
            .UseOpenApiDocumentation();
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers();
        builder.MapHealthCheck();
        return builder;
    }
}
