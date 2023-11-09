namespace Ok.Movies.MinimalAPI.Api.Endpoints.Movies;

public static class MovieEndpointExtensions
{
    public const string GroupName = "Movies";

    public static IEndpointRouteBuilder MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        return app
                .MapCreateMovie()
                .MapGetMovie()
                .MapGetAllMovies()
                .MapUpdateMovie()
                .MapDeleteMovie()
            ;
    }
}