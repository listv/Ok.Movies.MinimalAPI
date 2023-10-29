using Application.Repositories;
using Application.Validators;
using FluentValidation.TestHelper;
using NSubstitute;
using Xunit;

namespace Ok.Movies.Tests.Unit.Application.Validators;

public class MovieValidatorTests
{
    private readonly MovieValidator _sut;
    private readonly IMovieRepository _movieRepository;
    private readonly MovieFaker _movieFaker;

    public MovieValidatorTests()
    {
        _movieRepository = Substitute.For<IMovieRepository>();
        _sut = new MovieValidator(_movieRepository);
        _movieFaker = new MovieFaker();
    }

    [Fact]
    public async Task Validate_ShouldNotHaveValidationErrors_WhenModelIsValid()
    {
        // Arrange
        var movie = _movieFaker.Generate();

        // Act
        var result = await _sut.TestValidateAsync(movie);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForId_WhenIdIsEmpty()
    {
        // Arrange
        var model = _movieFaker.Clone()
            .RuleFor(movie => movie.Id, () => Guid.Empty)
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(movie => movie.Id);
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForGenres_WhenNoGenresProvided()
    {
        // Arrange
        var model = _movieFaker.Clone()
            .RuleFor(movie => movie.Genres, () => new List<string>())
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(movie => movie.Genres);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_ShouldHaveValidationErrorForTitle_WhenNoTitleProvided(string title)
    {
        // Arrange
        var model = _movieFaker.Clone()
            .RuleFor(movie => movie.Title, () => title)
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(movie => movie.Title);
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForYearOfRelease_WhenYearOfReleaseIsInFuture()
    {
        // Arrange
        var model = _movieFaker.Clone()
            .RuleFor(movie => movie.YearOfRelease, () => DateTime.UtcNow.Year + 1)
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(movie => movie.YearOfRelease);
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForSlug_WhenMovieWithGivenSlugExists()
    {
        // Arrange
        var model = _movieFaker.Generate();
        var model2 = _movieFaker.Generate();
        _movieRepository.GetBySlugAsync(model.Slug).Returns(model2);

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(movie => movie.Slug);
    }
}
