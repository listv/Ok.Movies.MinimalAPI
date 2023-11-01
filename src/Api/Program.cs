using Api;
using Api.Configurations;
using Application;
using Infrastructure;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;
using Serilog;

[assembly: ApiConventionType(typeof(MoviesApiConventions))]

StaticLogger.EnsureInitialized(args);
Log.Information("Server booting up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations().RegisterSerilog();

    builder.Services.AddControllers();
    builder.Services.AddInfrastructure(builder.Environment);
    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.UseInfrastructure();

    app.MapControllers();

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
