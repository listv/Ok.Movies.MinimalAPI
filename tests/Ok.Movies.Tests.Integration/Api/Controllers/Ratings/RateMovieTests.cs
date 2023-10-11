using System.Net;
using System.Net.Http.Json;
using Api;
using Bogus;
using FluentAssertions;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Ratings;

public class RateMovieTests:IClassFixture<RatingTestsFixture>
{
    private readonly RatingTestsFixture _fixture;
    private readonly RateMovieRequestFaker _requestFaker;

    public RateMovieTests(RatingTestsFixture fixture)
    {
        _fixture = fixture;
        _requestFaker = new RateMovieRequestFaker();
    }

    [Fact]
    public async Task RateMovie_ShouldReturnOk_WhenUserIsAuthenticatedAndMovieExists()
    {
        // Arrange
        var rateMovieRequest = _requestFaker.Generate();

        // Act
        var response = await _fixture.AuthenticatedClient
            .PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{_fixture.Movie!.Id}/ratings", rateMovieRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RateMovie_ShouldReturnNotFound_WhenMovieDoesNotExist()
    {
        // Arrange
        var rateMovieRequest = _requestFaker.Generate();

        // Act
        var response = await _fixture.AuthenticatedClient
            .PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{Guid.NewGuid()}/ratings", rateMovieRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(GetRandomNumbers))]
    public async Task RateMovie_ShouldReturnBadRequest_WhenDataIsInvalid(int invalidRatingValue)
    {
        var rateMovieRequest = _requestFaker.Clone()
            .RuleFor(request => request.Rating, () => invalidRatingValue)
            .Generate();

        // Act
        var response = await _fixture.AuthenticatedClient
            .PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{_fixture.Movie!.Id}/ratings", rateMovieRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RateMovie_ShouldReturnUnauthorized_WhenUserNotAuthorized()
    {
        // Arrange
        var rateMovieRequest = _requestFaker.Generate();

        // Act
        var response = await _fixture.UnauthorizedClient
            .PutAsJsonAsync($"{ApiEndpoints.Movies.Base}/{_fixture.Movie!.Id}/ratings", rateMovieRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public static IEnumerable<object[]> GetRandomNumbers()
    {
        yield return new object[] { new Faker().Random.Int(max: 0) };
        yield return new object[] { new Faker().Random.Int(min: 6) };
    }
}
