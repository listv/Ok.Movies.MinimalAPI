using Application.Models;
using Application.Repositories;
using Application.Services;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace Ok.Movies.Tests.Unit.Application.Services;

public class MovieServiceTests
{
    private readonly MovieService _sut;
    private readonly IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly IValidator<Movie> _movieValidator = Substitute.For<IValidator<Movie>>();

    private readonly IValidator<GetAllMoviesOptions> _moviesFilteringOptionsValidator =
        Substitute.For<IValidator<GetAllMoviesOptions>>();
    private readonly MovieFaker _movieFaker;

    public MovieServiceTests()
    {
        _sut = new MovieService(_movieRepository, _ratingRepository, _movieValidator, _moviesFilteringOptionsValidator);
        _movieFaker = new MovieFaker();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenMovieIsValid()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.CreateAsync(movie).Returns(true);

        // Act
        var result = await _sut.CreateAsync(movie);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFalse_WhenMovieIsValidButNotCreated()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.CreateAsync(movie).Returns(false);

        // Act
        var result = await _sut.CreateAsync(movie);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowExceptionAsync_WhenMovieIsInvalid()
    {
        // Arrange
        var invalidMovie = _movieFaker.Generate();
        _movieValidator.ValidateAndThrowAsync(invalidMovie)
            .ThrowsAsyncForAnyArgs(_ => throw new ValidationException(string.Empty));

        // Act
        var result = async () => await _sut.CreateAsync(invalidMovie);

        // Assert
        await result.Should().ThrowExactlyAsync<ValidationException>();
        await _movieRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<Movie>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMovie_WhenMovieExists()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.GetByIdAsync(Arg.Is(movie.Id)).Returns(movie);

        // Act
        var result = await _sut.GetByIdAsync(movie.Id);

        // Assert
        result.Should().BeEquivalentTo(movie);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMovieDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _movieRepository.GetByIdAsync(Arg.Is(id)).ReturnsNull();

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnMovie_WhenMovieExists()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.GetBySlugAsync(Arg.Is(movie.Slug)).Returns(movie);

        // Act
        var result = await _sut.GetBySlugAsync(movie.Slug);

        // Assert
        result.Should().BeEquivalentTo(movie);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnNull_WhenMovieDoesNotExist()
    {
        // Arrange
        _movieRepository.GetBySlugAsync(Arg.Any<string>()).ReturnsNull();

        // Act
        var result = await _sut.GetBySlugAsync(string.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMovies_WhenThereAreAny()
    {
        // Arrange
        var movies = _movieFaker.Generate(3);
        _movieRepository.GetAllAsync(Arg.Any<GetAllMoviesOptions>()).ReturnsForAnyArgs(movies);

        // Act
        var result = await _sut.GetAllAsync(new GetAllMoviesOptions());

        // Assert
        result.Should().BeEquivalentTo(movies);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenThereAreNoMovies()
    {
        // Arrange
        var movies = Enumerable.Empty<Movie>();
        _movieRepository.GetAllAsync(new GetAllMoviesOptions()).ReturnsForAnyArgs(movies);

        // Act
        var result = await _sut.GetAllAsync(new GetAllMoviesOptions());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowException_WhenFilteringOptionsInvalid()
    {
        // Arrange
        var invalidFilter = new GetAllMoviesOptions();
        _moviesFilteringOptionsValidator.ValidateAndThrowAsync(invalidFilter)
            .ThrowsAsyncForAnyArgs(_ => throw new ValidationException(string.Empty));

        // Act
        Func<Task<IEnumerable<Movie>>> func = async () => await _sut.GetAllAsync(invalidFilter);

        // Assert
        await func.Should().ThrowAsync<ValidationException>();
        await _movieRepository.DidNotReceiveWithAnyArgs().GetAllAsync(invalidFilter);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenMovieIsInvalid()
    {
        // Arrange
        var invalidMovie = _movieFaker.Generate();
        _movieValidator.ValidateAndThrowAsync(invalidMovie)
            .ThrowsAsyncForAnyArgs(_ => throw new ValidationException(string.Empty));

        // Act
        var result = async () => await _sut.UpdateAsync(invalidMovie);

        // Assert
        await result.Should().ThrowExactlyAsync<ValidationException>();
        await _movieRepository.DidNotReceive().UpdateAsync(Arg.Any<Movie>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenMovieDoesNotExist()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.ExistsByIdAsync(movie.Id).Returns(false);

        // Act
        var result = await _sut.UpdateAsync(movie);

        // Assert
        result.Should().BeNull();
        await _movieRepository.DidNotReceive().UpdateAsync(Arg.Any<Movie>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnMovie_WhenMovieExists()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.ExistsByIdAsync(movie.Id).Returns(true);

        // Act
        var result = await _sut.UpdateAsync(movie);

        // Assert
        result.Should().BeEquivalentTo(movie);
        await _movieRepository.Received(1).UpdateAsync(movie);
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldReturnTrue_WhenMovieExists()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.DeleteByIdAsync(movie.Id).Returns(true);

        // Act
        var result = await _sut.DeleteByIdAsync(movie.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldReturnFalse_WhenMovieDoesNotExist()
    {
        // Arrange
        var movie = _movieFaker.Generate();
        _movieRepository.DeleteByIdAsync(movie.Id).Returns(false);

        // Act
        var result = await _sut.DeleteByIdAsync(movie.Id);

        // Assert
        result.Should().BeFalse();
    }
}
