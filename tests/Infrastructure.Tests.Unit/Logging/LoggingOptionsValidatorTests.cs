using Bogus;
using FluentValidation.TestHelper;
using Infrastructure.Logging;
using Serilog.Events;
using Xunit;

namespace Infrastructure.Tests.Unit.Logging;

public class LoggingOptionsValidatorTests
{
    private readonly Faker<LoggingOptions> _optionsGenerator = new Faker<LoggingOptions>()
        .RuleFor(options => options.AppName, faker => faker.Lorem.Sentence(1, 2))
        .RuleFor(options => options.MinimumLogLevel, faker => faker.PickRandom<LogEventLevel>().ToString())
        .RuleFor(options => options.WriteToFile, faker => faker.Random.Bool())
        .RuleFor(options => options.StructuredConsoleLogging, faker => faker.Random.Bool());

    private readonly LoggingOptionsValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveValidationErrors_WhenModelIsValid()
    {
        LoggingOptions model = _optionsGenerator;

        _validator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveValidationErrorForAppName_WhenAppNameIsEmpty(string appName)
    {
        LoggingOptions model = _optionsGenerator.Clone().RuleFor(options => options.AppName, () => appName);

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(options => options.AppName);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForAppName_WhenAppNameIsTooLong()
    {
        const int maxAllowedWordLength = 255;
        LoggingOptions model = _optionsGenerator.Clone().RuleFor(options => options.AppName,
            faker => faker.Random.String(maxAllowedWordLength + 1, maxAllowedWordLength + 2));

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(options => options.AppName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveValidationerrorForMinimumLogLevel_WhenMinimumLogLevelIsEmpty(string minimumLogLevel)
    {
        LoggingOptions model = _optionsGenerator.Clone()
            .RuleFor(options => options.MinimumLogLevel, () => minimumLogLevel);

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(options => options.MinimumLogLevel);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForMinimumLogLevel_WhenMinimumLogLevelIsNotInEnum()
    {
        LoggingOptions model = _optionsGenerator.Clone()
            .RuleFor(options => options.MinimumLogLevel, faker => faker.Lorem.Word());

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(options => options.MinimumLogLevel);
    }

    [Fact]
    public void Validate_ShouldHaveValidationErrorForWriteToFile_WhenWriteToFileIsNotProvided()
    {
        LoggingOptions model = _optionsGenerator.Clone()
            .RuleFor(options => options.WriteToFile, () => null);

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(options => options.WriteToFile);
    }

    [Fact]
    public void
        Validate_ShouldHaveValidationErrorForStructuredConsoleLogging_WhenStructuredConsoleLoggingIsNotProvided()
    {
        LoggingOptions model = _optionsGenerator.Clone()
            .RuleFor(options => options.StructuredConsoleLogging, () => null);

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(options => options.StructuredConsoleLogging);
    }
}
