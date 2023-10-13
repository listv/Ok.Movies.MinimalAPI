using Application.Models;
using Application.Validators;
using Bogus;
using FluentValidation.TestHelper;
using Xunit;

namespace Ok.Movies.Tests.Unit.Application.Validators;

public class MoviesFilteringOptionsValidatorTests
{
    private readonly MoviesFilteringOptionsValidator _sut = new();

    [Fact]
    public async Task Validate_ShouldNotThrowExceptions_WhenModelIsValid()
    {
        // Arrange
        var model = new Faker<MoviesFilteringOptions>()
            .RuleFor(options => options.YearOfRelease, faker => faker.Date.Recent().Year).Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForYearOfRelease_WhenYearOfReleaseIsInThePast()
    {
        // Arrange
        var model = new Faker<MoviesFilteringOptions>()
            .RuleFor(options => options.YearOfRelease, faker => faker.Date.Future().Year).Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.YearOfRelease);
    }
}
