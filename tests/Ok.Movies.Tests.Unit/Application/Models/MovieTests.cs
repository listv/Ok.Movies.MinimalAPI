using Application.Models;
using Bogus;
using Fare;
using FluentAssertions;
using Xunit;

namespace Ok.Movies.Tests.Unit.Application.Models;

public class MovieTests
{
    private readonly Faker<Movie> _movieGenerator = new Faker<Movie>()
        .RuleFor(movie => movie.Id, faker => faker.Random.Guid())
        .RuleFor(movie => movie.Genres, faker => faker.Lorem.Words().ToList())
        .RuleFor(movie => movie.Title, faker => faker.Lorem.Word())
        .RuleFor(movie => movie.YearOfRelease, faker => faker.Date.Soon().Year);

    [Fact]
    public void Slug_ShouldRemoveRestrictedSymbols_WhenTheyPersistInTitle()
    {
        // Arrange
        var randomSymbolsGenerator = new Xeger("(.\\s){50}");
        var str = randomSymbolsGenerator.Generate();
        const string expected = "[0-9A-Za-z_-]"; //alphanumeric symbols, underscores and hyphens

        // Act
        var movie = _movieGenerator.Clone()
            .RuleFor(movie1 => movie1.Title, str)
            .Generate();

        // Assert
        movie.Slug.Should().MatchRegex(expected);
    }

    [Fact]
    public void Slug_ShouldEndWithYearOfRelease_Always()
    {
        var movie = _movieGenerator.Generate();

        movie.Slug.Should().EndWith($"-{movie.YearOfRelease}");
    }
}
