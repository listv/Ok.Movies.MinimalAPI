using Ok.Movies.MinimalAPI.Api.Mapping;
using Ok.Movies.MinimalAPI.Application.Services;
using Ok.Movies.MinimalAPI.Contracts.Responses;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.MinimalAPI.Infrastructure.Versioning;

namespace Ok.Movies.MinimalAPI.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async (
                CancellationToken token,
                HttpContext context,
                IRatingService ratingService) =>
            {
                var userId = context.GetUserId();
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);

                var ratingsResponse = ratings.MapToResponse();

                return TypedResults.Ok(ratingsResponse);
            })
            .WithName(Name)
            .WithTags(RatingEndpointExtensions.GroupName)
            .Produces<MovieRatingResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .WithApiVersionSet(ApiVersioning.VersionSet);

        return app;
    }
}