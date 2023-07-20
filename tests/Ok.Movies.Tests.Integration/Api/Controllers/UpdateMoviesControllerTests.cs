using System.Net;
using System.Net.Http.Json;
using Api;
using Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers;

public class UpdateMoviesControllerTests:IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;

    public UpdateMoviesControllerTests(TestApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Update_UpdatesMovie_WhenMovieExistsAndDataIsValid()
    {
        // Arrange
        var movie = MovieFaker.CreateMovieRequestGenerator().Generate();
        var createdResponse = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        movie = MovieFaker.CreateMovieRequestGenerator().Generate();

        // Act
        var response = await _client.PutAsJsonAsync($"{ApiEndpoints.Movies.Create}/{createdMovie!.Id}", movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customerResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        customerResponse!.Should().BeEquivalentTo(movie);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        // Arrange
        var movie = MovieFaker.CreateMovieRequestGenerator().Generate();

        // Act
        var response = await _client.PutAsJsonAsync($"{ApiEndpoints.Movies.Create}/{Guid.NewGuid()}", movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
