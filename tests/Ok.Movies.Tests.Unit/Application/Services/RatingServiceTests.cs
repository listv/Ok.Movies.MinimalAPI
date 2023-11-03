using Bogus;
using FluentAssertions;
using NSubstitute;
using Ok.Movies.Application.Models;
using Ok.Movies.Application.Repositories;
using Ok.Movies.Application.Services;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace Ok.Movies.Tests.Unit.Application.Services;

public class RatingServiceTests
{
    private readonly RatingService _sut;
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();
    private readonly Faker _faker;

    public RatingServiceTests()
    {
        _faker = new Faker();
        _sut = new RatingService(_ratingRepository, _movieRepository);
    }

    [Fact]
    public async Task RateMovieAsync_ShouldReturnTrue_WhenDataIsValidAndMovieExists()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var rating = _faker.Random.Int(1, 5);
        var userId = Guid.NewGuid();
        _movieRepository.ExistsByIdAsync(movieId).Returns(true);
        _ratingRepository.RateMovieAsync(movieId, rating, userId).Returns(true);

        // Act
        var result = await _ratingRepository.RateMovieAsync(movieId, rating, userId);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidRating))]
    public async Task RateMovieAsync_ShouldThrowException_WhenRatingIsInvalid(int rating)
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        Func<Task<bool>> result = async () =>
            await _sut.RateMovieAsync(movieId, rating, userId, Arg.Any<CancellationToken>());

        // Assert
        await result.Should().ThrowAsync<ValidationException>();
        await _movieRepository.DidNotReceiveWithAnyArgs().ExistsByIdAsync(Guid.Empty);
        await _ratingRepository.DidNotReceiveWithAnyArgs().RateMovieAsync(Guid.Empty, default, Guid.Empty);
    }

    [Fact]
    public async Task RateMovieAsync_ShouldReturnFalse_WhenMovieDoesNotExist()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var rating = _faker.Random.Int(1, 5);
        var userId = Guid.NewGuid();
        _movieRepository.ExistsByIdAsync(movieId).Returns(false);

        // Act
        var result = await _sut.RateMovieAsync(movieId, rating, userId);

        // Assert
        result.Should().BeFalse();
        await _ratingRepository.DidNotReceiveWithAnyArgs().RateMovieAsync(Guid.Empty, default, Guid.Empty);
    }

    [Fact]
    public async Task DeleteRatingAsync_ShouldDeleteRating_WhenRatingExists()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _ratingRepository.DeleteRatingAsync(movieId, userId).Returns(true);

        // Act
        var result = await _sut.DeleteRatingAsync(movieId, userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRatingAsync_ShouldReturnFalse_WhenRatingDoesNotExist()
    {
        // Arrange
        _ratingRepository.DeleteRatingAsync(Guid.Empty, Guid.Empty).ReturnsForAnyArgs(false);

        // Act
        var result = await _sut.DeleteRatingAsync(Guid.Empty, Guid.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetRatingsForUserAsync_ShouldReturnMovieRatings_WhenRatingsExist()
    {
        // Arrange
        var ratings = new Faker<MovieRating>().Generate(3);
        Guid userId = Guid.NewGuid();
        _ratingRepository.GetRatingsForUserAsync(userId).Returns(ratings);

        // Act
        var result = await _sut.GetRatingsForUserAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(ratings);
    }

    [Fact]
    public async Task GetRatingsForUserAsync_ShouldReturnEmptyCollection_WhenRatingsDoNotExist()
    {
        // Arrange
        var ratings = Enumerable.Empty<MovieRating>().ToList();
        Guid userId = Guid.NewGuid();
        _ratingRepository.GetRatingsForUserAsync(userId).ReturnsForAnyArgs(ratings);

        // Act
        var result = await _sut.GetRatingsForUserAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(ratings).And.BeEmpty();
    }

    public static IEnumerable<object[]> GetInvalidRating()
    {
        yield return new object[] { new Faker().Random.Int(max: 0) };
        yield return new object[] { new Faker().Random.Int(min: 6) };
    }
}
