using Api.Mapping;
using Application.Services;
using Contracts.Responses;
using Infrastructure.Authentication;
using Infrastructure.Versioning;

namespace Api.Endpoints.Ratings;

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