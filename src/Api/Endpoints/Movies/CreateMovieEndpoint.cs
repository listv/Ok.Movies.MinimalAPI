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

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create, async (
                CreateMovieRequest request,
                IMovieService movieService,
                IOutputCacheStore cacheStore,
                CancellationToken token) =>
            {
                var movie = request.MapToMovie();
                await movieService.CreateAsync(movie, token);
                await cacheStore.EvictByTagAsync(CacheExtensions.MoviesTag, token);
                var response = movie.MapToResponse();
                return TypedResults.CreatedAtRoute(response, GetMovieEndpoint.Name, new { idOrSlug = response.Id });
            })
            .WithName(Name)
            .WithTags(MovieEndpointExtensions.GroupName)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            .WithApiVersionSet(ApiVersioning.VersionSet);
        return app;
    }
}