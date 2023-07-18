using Api.Configurations;
using Application;
using Infrastructure.Logging;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("Server booting up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations()
        .RegisterSerilog();

    builder.Services.AddApplication();

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server shutting down...");
    Log.CloseAndFlush();
}
