using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Api;
using Bogus;
using Contracts.Requests;
using Contracts.Responses;
using FluentAssertions;
using Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

public class UpdateMoviesControllerTests:IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _apiFactory;

    private readonly CreateMovieRequestFaker _createMovieRequestFaker = new();

    public UpdateMoviesControllerTests(TestApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task Update_ShouldUpdatesMovie_WhenAuthenticatedAndAuthorizedAndMovieExistsAndDataIsValid()
    {
        // Arrange
        var movie = _createMovieRequestFaker.Generate();
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));
        var createdResponse = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        movie = _createMovieRequestFaker.Generate();

        // Act
        var response = await client.PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{createdMovie!.Id}", movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customerResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        customerResponse!.Should().BeEquivalentTo(movie);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var movie = _createMovieRequestFaker.Generate();
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{Guid.NewGuid()}", movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_ShouldReturnForbidden_WhenAuthenticatedButNotAuthorized()
    {
        // Arrange
        var movie = _createMovieRequestFaker.Generate();
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "false"));

        // Act
        var response = await client.PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{new Faker().Random.Guid()}", movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenAuthenticatedAndAuthorizedButDataIsInvalid()
    {
        // Arrange
        var movie = new UpdateMovieRequest
        {
            Title = string.Empty,
            YearOfRelease = 0,
            Genres = Enumerable.Empty<string>()
        };
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));

        // Act
        var response = await client.PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{Guid.NewGuid()}", movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenAuthenticatedAndAuthorizedButMovieDoesNotExist()
    {
        // Arrange
        var movie = _createMovieRequestFaker.Generate();
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));
    
        // Act
        var response = await client.PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{Guid.NewGuid()}", movie);
    
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
