using Bogus;
using FluentValidation.TestHelper;
using Ok.Movies.MinimalAPI.Application.Models;
using Ok.Movies.MinimalAPI.Application.Validators;
using Xunit;

namespace Ok.Movies.Tests.Unit.Application.Validators;

public class GetAllMoviesOptionsValidatorTests
{
    private readonly GetAllMoviesOptionsValidator _sut = new();

    private readonly Faker<GetAllMoviesOptions> _optionsFaker = new Faker<GetAllMoviesOptions>()
        .RuleFor(options => options.PageSize, faker => faker.Random.Int(1, 25))
        .RuleFor(options => options.Page, faker => faker.Random.Int(min: 1))
        .RuleFor(options => options.SortField,
            faker => faker.PickRandom(GetAllMoviesOptionsValidator.AcceptableSortFields))
        .RuleFor(options => options.YearOfRelease, faker => faker.Date.Past(10).Year);

    [Fact]
    public async Task Validate_ShouldNotThrowExceptions_WhenModelIsValid()
    {
        // Arrange
        var model = _optionsFaker.Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForYearOfRelease_WhenYearOfReleaseIsInFuture()
    {
        // Arrange
        var model = _optionsFaker.Clone()
            .RuleFor(o => o.YearOfRelease, () => DateTime.UtcNow.Year + 1)
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.YearOfRelease);
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForPage_WhenPageIsLessThenZero()
    {
        // Arrange
        var model = _optionsFaker.Clone()
            .RuleFor(options => options.Page, faker => faker.Random.Int(max: 0))
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.Page);
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIsLowerThanMinAllowedValue()
    {
        // Arrange
        var model = _optionsFaker.Clone()
            .RuleFor(options => options.PageSize, faker => faker.Random.Int(max: 0))
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.PageSize);
    }
    
    
    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForPageSize_WhenPageSizeGreaterThanMaxAllowedValue()
    {
        // Arrange
        var model = _optionsFaker.Clone()
            .RuleFor(options => options.PageSize, faker => faker.Random.Int(min: 26))
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.PageSize);
    }

    [Fact]
    public async Task Validate_ShouldHaveValidationErrorForSortField_WhenInvalidValueProvided()
    {
        // Arrange
        string sortBy;
        var faker = new Faker();
        do
        {
            sortBy = faker.Lorem.Word();
        } while (GetAllMoviesOptionsValidator.AcceptableSortFields.Contains(sortBy));

        var model = _optionsFaker.Clone()
            .RuleFor(options => options.SortField, () => sortBy)
            .Generate();

        // Act
        var result = await _sut.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.SortField);
    }
}
