using Api.Mapping;
using Application.Services;
using Contracts.Requests;
using Contracts.Responses;
using Infrastructure.Authentication;
using Infrastructure.Cache;
using Infrastructure.Versioning;

namespace Api.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetAllMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] GetAllMoviesRequest request,
                HttpContext context,
                IMovieService movieService,
                CancellationToken token) =>
            {
                var userId = context.GetUserId();

                var options = request.MapToOptions()
                    .WithUser(userId);

                var movies = await movieService.GetAllAsync(options, token);
                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

                var moviesResponse = movies.MapToResponse(
                    request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                    request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                    movieCount);

                return TypedResults.Ok(moviesResponse);
            })
            .WithName($"{Name}V1")
            .WithTags(MovieEndpointExtensions.GroupName)
            .Produces<MoviesResponse>()
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);

        app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] GetAllMoviesRequest request,
                HttpContext context,
                IMovieService movieService,
                CancellationToken token) =>
            {
                var userId = context.GetUserId();

                var options = request.MapToOptions()
                    .WithUser(userId);

                var movies = await movieService.GetAllAsync(options, token);
                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

                var moviesResponse = movies.MapToResponse(
                    request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                    request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                    movieCount);

                return TypedResults.Ok(moviesResponse);
            })
            .WithName($"{Name}V2")
            .WithTags(MovieEndpointExtensions.GroupName)
            .Produces<MoviesResponse>()
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(2.0)
            .CacheOutput(CacheExtensions.MovieCachePolicy);

        return app;
    }
}
