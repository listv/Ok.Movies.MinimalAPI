using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ok.Movies.MinimalAPI.Infrastructure.Versioning;

public static class ApiVersioning
{
    public static ApiVersionSet VersionSet { get; private set; }

    public static IEndpointRouteBuilder CreateApiVersionSet(this IEndpointRouteBuilder app)
    {
        VersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .HasApiVersion(new ApiVersion(2.0))
            .ReportApiVersions()
            .Build();

        return app;
    }
}