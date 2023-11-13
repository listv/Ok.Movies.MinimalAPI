using Microsoft.AspNetCore.OutputCaching;
using Ok.Movies.MinimalAPI.Application.Services;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.MinimalAPI.Infrastructure.Cache;
using Ok.Movies.MinimalAPI.Infrastructure.Versioning;

namespace Ok.Movies.MinimalAPI.Api.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public const string Name = "DeleteMovie";

    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.Delete, async (
                Guid id,
                IMovieService movieService,
                IOutputCacheStore cacheStore,
                CancellationToken token) =>
            {
                var isDeleted = await movieService.DeleteByIdAsync(id, token);
                if (!isDeleted) return Results.NotFound();
                await cacheStore.EvictByTagAsync(CacheExtensions.MoviesTag, token);
                return Results.Ok();
            })
            .WithName(Name)
            .WithTags(MovieEndpointExtensions.GroupName)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.AdminUserPolicyName)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .IsApiVersionNeutral();

        return app;
    }
}
