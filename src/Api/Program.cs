using Microsoft.AspNetCore.Mvc;
using Ok.Movies.MinimalAPI.Api.Configurations;
using Ok.Movies.MinimalAPI.Api.Controllers;
using Ok.Movies.MinimalAPI.Application;
using Ok.Movies.MinimalAPI.Infrastructure;
using Ok.Movies.MinimalAPI.Infrastructure.Logging;
using Serilog;

[assembly: ApiConventionType(typeof(ApiConventions))]

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

    app.UseInfrastructure();
    app.MapEndpoints();

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
