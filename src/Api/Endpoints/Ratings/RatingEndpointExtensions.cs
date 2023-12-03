namespace Api.Endpoints.Ratings;

public static class RatingEndpointExtensions
{
    public const string GroupName = "Ratings";

    public static IEndpointRouteBuilder MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        return app
                .MapRateMovie()
                .MapDeleteRating()
                .MapGetUserRatings()
            ;
    }
}