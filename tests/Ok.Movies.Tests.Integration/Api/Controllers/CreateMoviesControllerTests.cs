using System.Net;
using System.Net.Http.Json;
using Bogus;
using Contracts.Requests;
using Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Integration.Api.Controllers;

public class CreateMoviesControllerTests : IClassFixture<MoviesApiFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<CreateMovieRequest> _movieGenerator = new Faker<CreateMovieRequest>()
        .RuleFor(request => request.Title, faker => faker.Lorem.Sentence())
        .RuleFor(request => request.Genres, faker => faker.Lorem.Words())
        .RuleFor(request => request.YearOfRelease, faker => faker.Date.Soon().Year);

    public CreateMoviesControllerTests(MoviesApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Create_CreatesMovie_WhenDataIsValid()
    {
        // Arrange
        var movie = _movieGenerator.Generate();

        // Act
        var response = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movieResponse.Should().BeEquivalentTo(movie);
        response.Headers.Location!.ToString().Should().Be($"/{ApiEndpoints.Movies.Create}/{movieResponse!.Id}");
    }
}
