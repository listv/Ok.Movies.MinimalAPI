using FluentValidation;
using Serilog.Events;

namespace Ok.Movies.Infrastructure.Logging;

public class LoggingOptionsValidator:AbstractValidator<LoggingOptions>
{
    public LoggingOptionsValidator()
    {
        RuleFor(options => options.AppName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(options => options.MinimumLogLevel)
            .NotEmpty()
            .IsEnumName(typeof(LogEventLevel));
        
        RuleFor(options => options.StructuredConsoleLogging)
            .NotEmpty();

        RuleFor(options => options.WriteToFile)
            .NotEmpty();
    }
}
