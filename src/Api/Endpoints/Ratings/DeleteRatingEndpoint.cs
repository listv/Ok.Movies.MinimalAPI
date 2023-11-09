using Ok.Movies.MinimalAPI.Application.Services;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.MinimalAPI.Infrastructure.Versioning;

namespace Ok.Movies.MinimalAPI.Api.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    public const string Name = "DeleteRating";

    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (
                Guid id,
                HttpContext context,
                IRatingService ratingService,
                CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var isRatingDeleted = await ratingService.DeleteRatingAsync(id, userId!.Value, token);

                return isRatingDeleted ? Results.Ok() : Results.NotFound();
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