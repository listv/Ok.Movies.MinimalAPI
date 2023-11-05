using System.Net;
using FluentAssertions;
using Ok.Movies.MinimalAPI.Api;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Ratings;

public class DeleteRatingTests: IClassFixture<RatingTestsFixture>
{
    private readonly RatingTestsFixture _fixture;

    public DeleteRatingTests(RatingTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeleteRating_ShouldReturnOk_WhenAuthorizedAndRatingExists()
    {
        // Arrange
        var ratedMovie = await _fixture.CreateRatedMovieAsync();
        
        // Act
        var response = await _fixture.AuthenticatedClient.DeleteAsync(
            $"{ApiEndpoints.Movies.Base}/{ratedMovie!.Id}/ratings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteRating_ShouldReturnNotFound_WhenAuthorizedAndRatingsDoNotExist()
    {
        // Arrange
        
        // Act
        var response = await _fixture.AuthenticatedClient.DeleteAsync(
            $"{ApiEndpoints.Movies.Base}/{_fixture.Movie!.Id}/ratings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRating_ShouldReturnUnauthorized_WhenUnauthorizedAndRatingsExist()
    {
        // Arrange
        var ratedMovie = await _fixture.CreateRatedMovieAsync();
        
        // Act
        var response = await _fixture.UnauthorizedClient.DeleteAsync(
            $"{ApiEndpoints.Movies.Base}/{ratedMovie!.Id}/ratings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
