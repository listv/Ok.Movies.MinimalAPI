using Microsoft.AspNetCore.Mvc;
using Ok.Movies.Api.Configurations;
using Ok.Movies.Api.Controllers;
using Ok.Movies.Application;
using Ok.Movies.Infrastructure;
using Ok.Movies.Infrastructure.Logging;
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
