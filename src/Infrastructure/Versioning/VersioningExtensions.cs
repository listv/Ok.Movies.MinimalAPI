using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Versioning;

public static class VersioningExtensions
{
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1.0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
                x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
            })
            .AddApiExplorer();

        return services;
    }

    public static IApplicationBuilder UseVersioning(this IApplicationBuilder app)
    {
        (app as IEndpointRouteBuilder)?.CreateApiVersionSet();
        return app;
    }
}