using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Api;
using Bogus;
using Contracts.Responses;
using FluentAssertions;
using Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

public class GetAllMoviesControllerTests:IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;
    private readonly TestApiFactory _apiFactory;

    public GetAllMoviesControllerTests(TestApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsAllMovies_WhenMoviesExist()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var authorizedClient = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));
        
        var createdResponse = await authorizedClient.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await _client.GetAsync(ApiEndpoints.Movies.GetAll);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Single().Should().BeEquivalentTo(createdMovie);
        
        // Cleanup resources
        var deleteClient = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.AdminUserClaimName, "true"));
        await deleteClient.DeleteAsync($"{ApiEndpoints.Movies.Create}/{createdMovie!.Id}");
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyItems_WhenNoMoviesExist()
    {
        var response = await _client.GetAsync(ApiEndpoints.Movies.GetAll);
    
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenInvalidFilterProvided()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{ApiEndpoints.Movies.GetAll}?year={new Faker().Date.Future().Year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
