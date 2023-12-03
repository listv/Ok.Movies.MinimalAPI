using FluentValidation;
using FluentValidation.TestHelper;
using Ok.Movies.MinimalAPI.Infrastructure.Cache;
using Ok.Movies.Tests.Unit.Core;
using Xunit;

namespace Ok.Movies.Tests.Unit.Infrastructure.Cache;

public class RedisCacheOptionsValidatorTests
{
    private readonly IValidator<RedisCacheOptions> _sut = new RedisCacheOptionsValidator();

    [Fact]
    public void Validate_ShouldNotHaveAnyValidationErrors_WhenConnectionStringIsValid()
    {
        // Arrange
        var model = new RedisCacheOptions
        {
            ConnectionString = "localhost"
        };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetEmptyStrings), MemberType = typeof(TestDataGenerator))]
    public void Validate_ShouldHaveValidationErrorForConnectionString_WhenConnectionStringIsEmpty(string connStr)
    {
        // Arrange
        var model = new RedisCacheOptions{ ConnectionString = connStr};

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.ConnectionString);
    }
}
