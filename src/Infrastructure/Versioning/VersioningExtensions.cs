using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Ok.Movies.MinimalAPI.Infrastructure.Versioning;

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
            .AddMvc()
            .AddApiExplorer();

        return services;
    }
}
