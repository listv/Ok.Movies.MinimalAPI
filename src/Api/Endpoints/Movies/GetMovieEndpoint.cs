using Api.Mapping;
using Application.Services;
using Contracts.Responses;
using Infrastructure.Authentication;
using Infrastructure.Cache;
using Infrastructure.Versioning;

namespace Api.Endpoints.Movies;

public static class GetMovieEndpoint
{
    public const string Name = "GetMovie";

    public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.Get, async (
                string idOrSlug,
                HttpContext context,
                IMovieService movieService,
                CancellationToken token) =>
            {
                var user = context.GetUserId();

                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, user, token)
                    : await movieService.GetBySlugAsync(idOrSlug, user, token);

                if (movie is null) return Results.NotFound();

                var movieResponse = movie.MapToResponse();
                return TypedResults.Ok(movieResponse);
            })
            .WithName(Name)
            .WithTags(MovieEndpointExtensions.GroupName)
            .Produces<MovieResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .IsApiVersionNeutral()
            .CacheOutput(CacheExtensions.MovieCachePolicy)
            .IsApiVersionNeutral();

        return app;
    }
}
