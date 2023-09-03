using Fare;
using FluentAssertions;
using Xunit;

namespace Ok.Movies.Tests.Unit.Application.Models;

public class MovieTests
{
    private readonly MovieFaker _movieFaker = new();

    [Fact]
    public void Slug_ShouldRemoveRestrictedSymbols_WhenTheyPersistInTitle()
    {
        // Arrange
        var randomSymbolsGenerator = new Xeger("(.\\s){50}");
        var str = randomSymbolsGenerator.Generate();
        const string expected = "[0-9A-Za-z_-]"; //alphanumeric symbols, underscores and hyphens

        // Act
        var movie = _movieFaker.Clone()
            .RuleFor(movie1 => movie1.Title, str)
            .Generate();

        // Assert
        movie.Slug.Should().MatchRegex(expected);
    }

    [Fact]
    public void Slug_ShouldEndWithYearOfRelease_Always()
    {
        var movie = _movieFaker.Generate();

        movie.Slug.Should().EndWith($"-{movie.YearOfRelease}");
    }
}
