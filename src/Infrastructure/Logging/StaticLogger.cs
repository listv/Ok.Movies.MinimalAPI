using Serilog;
using Serilog.Events;

namespace Infrastructure.Logging;

public static class StaticLogger
{
    public static void EnsureInitialized(IEnumerable<string> args)
    {
        if (Log.Logger is Serilog.Core.Logger) return;
        
        var isIntegrationTest = args.Contains("--integrationTest=true");
        Log.Logger = isIntegrationTest 
            ? CreateIntegrationTestLogger()
            : CreateBootstrapLogger();
    }

    private static ILogger CreateIntegrationTestLogger()
        => SetupLoggerConfiguration()
            .CreateLogger();

    private static ILogger CreateBootstrapLogger()
        => SetupLoggerConfiguration()
            .CreateBootstrapLogger();

    private static LoggerConfiguration SetupLoggerConfiguration() =>
        new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console();
}
