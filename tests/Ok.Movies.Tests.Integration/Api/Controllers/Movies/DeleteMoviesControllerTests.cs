using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using Ok.Movies.Api;
using Ok.Movies.Contracts.Responses;
using Ok.Movies.Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

public class DeleteMoviesControllerTests:IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _apiFactory;

    public DeleteMoviesControllerTests(TestApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenAuthenticatedAndAuthorizedAndMovieExists()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.AdminUserClaimName, "true"));
        var createdResponse = await client.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{createdMovie!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenAuthenticatedButNotAuthorized()
    {
        // Arrange
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.AdminUserClaimName, "false"));

        // Act
        var response = await client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenAuthenticatedAndAuthorizedButMovieDoesNotExist()
    {
        var client = _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.AdminUserClaimName, "true"));
        
        var response = await client.DeleteAsync($"{ApiEndpoints.Movies.Create}/{Guid.NewGuid()}");
    
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
