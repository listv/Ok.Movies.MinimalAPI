using Api.Endpoints.Movies;
using Api.Endpoints.Ratings;

namespace Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapMovieEndpoints()
            .MapRatingEndpoints();
    }
}