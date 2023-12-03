using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

        return services;
    }

    public static IApplicationBuilder MapHealthCheck(this IApplicationBuilder app)
    {
        (app as IEndpointRouteBuilder)?.MapHealthChecks("_health");
        return app;
    }
}