using Application.Models;
using Application.Repositories;
using Application.Services;
using Bogus;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Ok.Movies.Tests.Unit.Application.Services;

public class MovieServiceTests
{
    private readonly MovieService _sut;
    private readonly IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();
    private readonly MovieFaker _movieFaker;

    public MovieServiceTests()
    {
        _sut = new MovieService(_movieRepository);
        _movieFaker = new MovieFaker();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenMovieCreated()
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
    public async Task CreateAsync_ShouldReturnFalse_WhenSomethingWentWrong()
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
        var slug = new Faker().Lorem.Slug();
        _movieRepository.GetBySlugAsync(Arg.Is(slug)).ReturnsNull();

        // Act
        var result = await _sut.GetBySlugAsync(slug);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMovies_WhenThereAreAny()
    {
        // Arrange
        var movies = _movieFaker.Generate(3);
        _movieRepository.GetAllAsync().Returns(movies);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(movies);
    }
    
    

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenThereAreNoMovies()
    {
        // Arrange
        var movies = Enumerable.Empty<Movie>();
        _movieRepository.GetAllAsync().Returns(movies);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
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
