using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Api;
using Contracts.Requests;
using Contracts.Responses;
using FluentAssertions;
using Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

public class CreateMoviesControllerTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _apiFactory;

    public CreateMoviesControllerTests(TestApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task Create_ShouldCreateMovie_WhenAuthenticatedAndAuthorizedAndDataIsValid()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));

        // Act
        var response = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movieResponse.Should().BeEquivalentTo(movie);
        response.Headers.Location!.ToString().Should()
            .Be($"http://localhost/{ApiEndpoints.Movies.Create}/{movieResponse!.Id}");
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ShouldReturnForbidden_WhenAuthenticatedButNotAuthorized()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "false"));

        // Act
        var response = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }



    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenAuthenticatedAndAuthorizedButDataIsInvalid()
    {
        // Arrange
        var movie = new CreateMovieRequest
        {
            Title = string.Empty,
            YearOfRelease = 0,
            Genres = Enumerable.Empty<string>()
        };
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));

        // Act
        var response = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
