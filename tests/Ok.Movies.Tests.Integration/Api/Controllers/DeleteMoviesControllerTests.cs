using System.Net;
using System.Net.Http.Json;
using Api;
using Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers;

public class DeleteMoviesControllerTests:IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;

    public DeleteMoviesControllerTests(TestApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenMovieExists()
    {
        // Arrange
        var movie = MovieFaker.CreateMovieRequestGenerator().Generate();
        var createdResponse = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await _client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{createdMovie!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        var response = await _client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
