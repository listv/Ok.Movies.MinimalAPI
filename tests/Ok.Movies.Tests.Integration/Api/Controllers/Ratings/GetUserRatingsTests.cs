using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using Ok.Movies.Api;
using Ok.Movies.Contracts.Responses;
using Ok.Movies.Infrastructure.Authentication;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Ratings;

public class GetUserRatingsTests: IClassFixture<RatingTestsFixture>
{
    private readonly RatingTestsFixture _fixture;

    public GetUserRatingsTests(RatingTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetUserRatings_ShouldReturnEmptyRatings_WhenAuthorizedAndNoRatingsExist()
    {
        // Arrange
        var client = _fixture.AuthenticatedClient;

        // Act
        var response = await client.GetAsync($"{ApiEndpoints.Ratings.GetUserRatings}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var ratingResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MovieRatingResponse>>();
        ratingResponse.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserRatings_ShouldReturnUserRatings_WhenAuthorizedAndRatedMoviesExist()
    {
        // Arrange
        var ratedMovie = await _fixture.CreateRatedMovieAsync();
        
        var expected = new List<MovieRatingResponse>
        {
            new()
            {
                MovieId = ratedMovie!.Id,
                Rating = ratedMovie.UserRating!.Value,
                Slug = ratedMovie.Slug
            }
        };

        // Act
        var response = await _fixture.AuthenticatedClient.GetAsync($"{ApiEndpoints.Ratings.GetUserRatings}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var ratingResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MovieRatingResponse>>();
        ratingResponse.Should().ContainSingle().And.BeEquivalentTo(expected);

        var adminClient = _fixture.CreateAndConfigureClient(new Claim(AuthConstants.AdminUserClaimName, "true"));
        await adminClient.DeleteAsync($"{ApiEndpoints.Movies.Base}/{ratedMovie.Id}");
    }
}
