using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Ok.Movies.MinimalAPI.Infrastructure.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

        return services;
    }

    public static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("_health");
}
