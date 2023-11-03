namespace Ok.Movies.Infrastructure.Logging;

public class LoggingOptions
{
    public const string SectionName = "Logging";
    
    public required bool? WriteToFile { get; init; }
    public required bool? StructuredConsoleLogging { get; init; }
    public required string? MinimumLogLevel { get; init; }
    public required string? AppName { get; init; }
}
