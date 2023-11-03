using Bogus;
using FluentValidation;
using FluentValidation.TestHelper;
using Ok.Movies.Infrastructure.Authentication;
using Ok.Movies.Tests.Unit.Core;
using Xunit;

namespace Ok.Movies.Tests.Unit.Infrastructure.Authentication;

public class JwtOptionsValidatorTests
{
    private readonly IValidator<JwtOptions> _sut;
    private readonly Faker<JwtOptions> _optionsGenerator;

    public JwtOptionsValidatorTests()
    {
        _sut = new JwtOptionsValidator();
        _optionsGenerator = new Faker<JwtOptions>()
            .RuleFor(options => options.Audience, faker => faker.Internet.Url())
            .RuleFor(options => options.Key, faker => faker.Internet.Password())
            .RuleFor(options => options.Issuer, faker => faker.Internet.Url());
    }

    [Fact]
    public void Validate_ShouldNotHaveAnyValidationErrors_WhenOptionsAreValid()
    {
        // Arrange
        var model = _optionsGenerator.Generate();

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetEmptyStrings), MemberType = typeof(TestDataGenerator))]
    public void Validate_ShouldHaveValidationErrorForAudience_WhenAudienceIsEmpty(string audience)
    {
        // Arrange
        var model = _optionsGenerator.Clone().RuleFor(options => options.Audience, audience);

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.Audience);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForAudience_WhenAudienceIsTooLong()
    {
        // Arrange
        var model = _optionsGenerator.Clone()
            .RuleFor(x => x.Audience, faker => faker.Lorem.Letter(MaxAllowedLength + 1));

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Audience);
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetEmptyStrings), MemberType = typeof(TestDataGenerator))]
    public void Validate_ShouldHaveValidationErrorForKey_WhenKeyIsEmpty(string key)
    {
        // Arrange
        var model = _optionsGenerator.Clone().RuleFor(options => options.Key, key);

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.Key);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForKey_WhenKeyIsTooLong()
    {
        // Arrange
        var model = _optionsGenerator.Clone().RuleFor(x => x.Key, faker => faker.Lorem.Letter(MaxAllowedLength + 1));

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetEmptyStrings), MemberType = typeof(TestDataGenerator))]
    public void Validate_ShouldHaveValidationErrorForIssuer_WhenIssuerIsEmpty(string issuer)
    {
        // Arrange
        var model = _optionsGenerator.Clone().RuleFor(options => options.Issuer, issuer);

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(options => options.Issuer);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForIssuer_WhenIssuerIsTooLong()
    {
        // Arrange
        var model = _optionsGenerator.Clone().RuleFor(x => x.Issuer, faker => faker.Lorem.Letter(MaxAllowedLength + 1));

        // Act
        var result = _sut.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Issuer);
    }

    private static int MaxAllowedLength => 255;
}
