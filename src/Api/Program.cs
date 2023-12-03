using Api.Configurations;
using Api.Endpoints;
using Application;
using Infrastructure;
using Infrastructure.Logging;
using Infrastructure.Versioning;
using Serilog;

StaticLogger.EnsureInitialized(args);
Log.Information("Server booting up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations().RegisterSerilog();

    builder.Services.AddInfrastructure(builder.Environment);
    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseVersioning();
    app.MapApiEndpoints();
    app.UseInfrastructure();

    app.Run();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized(args);
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    StaticLogger.EnsureInitialized(args);
    Log.Information("Server shutting down...");
    Log.CloseAndFlush();
}
