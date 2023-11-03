using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ok.Movies.Infrastructure.Middleware;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services, IHostEnvironment environment)
    {
        return services.AddProblemDetails(
            options =>
            {
                options.IncludeExceptionDetails = (_, _) => environment.IsDevelopment();

                options.MapFluentValidationException();
            });
    }

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseProblemDetails();
}
