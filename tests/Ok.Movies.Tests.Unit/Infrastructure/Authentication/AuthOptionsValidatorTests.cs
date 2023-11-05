using Bogus;
using FluentValidation;
using FluentValidation.TestHelper;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.Tests.Unit.Core;
using Xunit;

namespace Ok.Movies.Tests.Unit.Infrastructure.Authentication;

public class AuthOptionsValidatorTests
{
    private readonly IValidator<AuthOptions> _sut = new AuthOptionsValidator();

    [Fact]
    public void Validate_ShouldNotHaveAnyValidationErrors_WhenModelIsValid()
    {
        // Arrange
        var model = new AuthOptions { ApiKey = Guid.NewGuid().ToString() };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetEmptyStrings), MemberType = typeof(TestDataGenerator))]
    public void Validate_ShouldHaveValidationErrorForApiKey_WhenApiKeyIsEmpty(string apiKey)
    {
        // Arrange
        var model = new AuthOptions { ApiKey = apiKey };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.ApiKey);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForApiKey_WhenApiKeyIsInvalid()
    {
        // Arrange
        var model = new AuthOptions { ApiKey = new Faker().Lorem.Word() };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.ApiKey);
    }
}
