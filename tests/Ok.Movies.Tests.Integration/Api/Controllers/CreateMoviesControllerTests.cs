using System.Net;
using System.Net.Http.Json;
using Api;
using Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers;

public class CreateMoviesControllerTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;

    public CreateMoviesControllerTests(TestApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Create_CreatesMovie_WhenDataIsValid()
    {
        // Arrange
        var movie = MovieFaker.CreateMovieRequestGenerator().Generate();

        // Act
        var response = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movieResponse.Should().BeEquivalentTo(movie);
        response.Headers.Location!.ToString().Should().Be($"http://localhost/{ApiEndpoints.Movies.Create}/{movieResponse!.Id}");
    }
}
