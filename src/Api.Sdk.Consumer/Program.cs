using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Ok.Movies.MinimalAPI.Api.Sdk;
using Ok.Movies.MinimalAPI.Api.Sdk.Consumer;
using Ok.Movies.MinimalAPI.Contracts.Requests;
using Refit;

var services = new ServiceCollection();
services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(x => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (_, cancellationToken) =>
            await x.GetRequiredService<AuthTokenProvider>().GetTokenAsync(cancellationToken)
    })
    .ConfigureHttpClient(x =>
    {
        x.BaseAddress = new Uri("https://localhost:5000");
    });
var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new []{ "Action"}
});

var movie = await moviesApi.GetMovieAsync(newMovie.Id.ToString());
Console.WriteLine($"Get movie by id:\n{JsonSerializer.Serialize(movie)}");

movie = await moviesApi.GetMovieAsync(newMovie.Slug);
Console.WriteLine($"Get movie by slug:\n{JsonSerializer.Serialize(movie)}");

await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new []{ "Action", "Adventure"}
});
movie = await moviesApi.GetMovieAsync(newMovie.Id.ToString());
Console.WriteLine($"Update movie:\n{JsonSerializer.Serialize(movie)}");

var getAllMoviesRequest = new GetAllMoviesRequest
{
    Page = 1,
    PageSize = 3,
    Title = null,
    Year = null,
    SortBy = null
};
var movies = await moviesApi.GetMoviesAsync(getAllMoviesRequest);
Console.WriteLine($"Get all movies:\n{JsonSerializer.Serialize(movies)}");

var rateRequest = new RateMovieRequest
{
    Rating = 5
};
await moviesApi.RateMovieAsync(movie.Id, rateRequest);
movie = await moviesApi.GetMovieAsync(newMovie.Id.ToString());
Console.WriteLine($"Rate movie:\n{JsonSerializer.Serialize(movie)}");

var userRatings = await moviesApi.GetUserRatingsAsync();
Console.WriteLine($"Get user ratings:\n{JsonSerializer.Serialize(userRatings)}");

await moviesApi.DeleteRatingAsync(movie.Id);
movie = await moviesApi.GetMovieAsync(movie.Id.ToString());
Console.WriteLine($"Delete rating:\n{JsonSerializer.Serialize(movie)}");

await moviesApi.DeleteMovieAsync(movie.Id);
movie = await moviesApi.GetMovieAsync(movie.Id.ToString());
Console.WriteLine($"Delete movie:\n{JsonSerializer.Serialize(movie)}");

