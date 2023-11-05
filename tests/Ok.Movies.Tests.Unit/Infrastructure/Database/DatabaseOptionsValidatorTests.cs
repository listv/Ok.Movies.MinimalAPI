using FluentValidation;
using FluentValidation.TestHelper;
using Ok.Movies.MinimalAPI.Infrastructure.Database;
using Xunit;

namespace Ok.Movies.Tests.Unit.Infrastructure.Database;

public class DatabaseOptionsValidatorTests
{
    private readonly IValidator<DatabaseOptions> _sut = new DatabaseOptionsValidator();

    [Fact]
    public void Validate_ShouldNotHaveAnyValidationErrors_WhenConnectionStringValid()
    {
        // Arrange
        var model = new DatabaseOptions
        {
            ConnectionString = "Server=localhost;database=movies;Port=5432;User ID=postgres;Password=Passw0rd;"
        };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldHaveValidationErrorForConnectionString_WhenConnectionStringIsEmpty(string connStr)
    {
        // Arrange
        var model = new DatabaseOptions { ConnectionString = connStr };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.ConnectionString)
            .WithErrorMessage("'Connection String' must not be empty.");
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForConnectionString_WhenConnectionStringIsInvalid()
    {
        // Arrange
        var model = new DatabaseOptions { ConnectionString = "invalidConnectionString" };

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.ConnectionString)
            .WithErrorMessage("Invalid 'Connection String' value provided: 'invalidConnectionString'.");
    }
}
