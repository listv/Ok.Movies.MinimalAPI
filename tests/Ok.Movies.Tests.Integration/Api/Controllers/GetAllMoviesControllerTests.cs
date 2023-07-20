using System.Net;
using System.Net.Http.Json;
using Api;
using Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers;

public class GetAllMoviesControllerTests:IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;

    public GetAllMoviesControllerTests(TestApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsAllMovies_WhenMoviesExist()
    {
        // Arrange
        var movie = MovieFaker.CreateMovieRequestGenerator().Generate();
        var createdResponse = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await _client.GetAsync(ApiEndpoints.Movies.GetAll);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Single().Should().BeEquivalentTo(createdMovie);
        
        // Cleanup resources
        await _client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{createdMovie!.Id}"); // TODO: Should be deleted after real DB implementation???
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyItems_WhenNoMoviesExist()
    {
        var response = await _client.GetAsync(ApiEndpoints.Movies.GetAll);
    
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeEmpty();
    }
}
