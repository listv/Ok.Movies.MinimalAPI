using Ok.Movies.MinimalAPI.Api.Endpoints.Movies;
using Ok.Movies.MinimalAPI.Api.Endpoints.Ratings;

namespace Ok.Movies.MinimalAPI.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapMovieEndpoints()
            .MapRatingEndpoints();
    }
}