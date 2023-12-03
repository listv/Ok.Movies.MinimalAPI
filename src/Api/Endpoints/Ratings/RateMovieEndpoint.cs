using Application.Services;
using Contracts.Requests;
using Infrastructure.Authentication;
using Infrastructure.Versioning;

namespace Api.Endpoints.Ratings;

public static class RateMovieEndpoint
{
    public const string Name = "RateMovie";

    public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Rate, async (
                Guid id,
                RateMovieRequest request,
                HttpContext context,
                IRatingService ratingService,
                CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var isMovieUpdated = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);

                return isMovieUpdated ? Results.Ok() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(RatingEndpointExtensions.GroupName)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .WithApiVersionSet(ApiVersioning.VersionSet);

        return app;
    }
}