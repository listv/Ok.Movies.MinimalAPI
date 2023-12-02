using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Ok.Movies.MinimalAPI.Api.Mapping;
using Ok.Movies.MinimalAPI.Application.Services;
using Ok.Movies.MinimalAPI.Contracts.Requests;
using Ok.Movies.MinimalAPI.Contracts.Responses;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.MinimalAPI.Infrastructure.Cache;
using Ok.Movies.MinimalAPI.Infrastructure.Versioning;

namespace Ok.Movies.MinimalAPI.Api.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update, async (
                Guid id,
                UpdateMovieRequest request,
                HttpContext context,
                IMovieService movieService,
                IOutputCacheStore cacheStore,
                CancellationToken token) =>
            {
                var user = context.GetUserId();

                var movie = request.MapToMovie(id);
                var updatedMovie = await movieService.UpdateAsync(movie, user, token);
                if (updatedMovie is null) return Results.NotFound();

                await cacheStore.EvictByTagAsync(CacheExtensions.MoviesTag, token);

                var response = updatedMovie.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(MovieEndpointExtensions.GroupName)
            .Produces<MovieResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .IsApiVersionNeutral();

        return app;
    }
}
