using System.Net.Http.Json;
using System.Security.Claims;
using Ok.Movies.Api;
using Ok.Movies.Contracts.Responses;
using Ok.Movies.Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Ratings;

public class RatingTestsFixture : TestApiFactory
{
    private MovieResponse? _movie;
    private HttpClient? _unauthorizedClient;
    private HttpClient? _authenticatedClient;

    public MovieResponse? Movie => _movie ??= CreateMovieAsync().GetAwaiter().GetResult();

    public HttpClient UnauthorizedClient => _unauthorizedClient ??= CreateClient();

    public HttpClient AuthenticatedClient => _authenticatedClient ??= CreateAndConfigureClient();

    public async Task<MovieResponse?> CreateRatedMovieAsync()
    {
        var movie = await CreateMovieAsync();

        return await RateMovieAsync(movie);
    }

    private async Task<MovieResponse?> CreateMovieAsync()
    {
        var client = CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));

        var createMovieRequest = new CreateMovieRequestFaker().Generate();
        var createdResponse = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, createMovieRequest);

        return await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();
    }

    private async Task<MovieResponse?> RateMovieAsync(MovieResponse? movie)
    {
        var rateMovieRequest = new RateMovieRequestFaker().Generate();

        await AuthenticatedClient.PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{movie!.Id}/ratings",
            rateMovieRequest);

        var response = await AuthenticatedClient.GetAsync($"{ApiEndpoints.Movies.Base}/{movie.Id}");
        return await response.Content.ReadFromJsonAsync<MovieResponse>();
    }
}
