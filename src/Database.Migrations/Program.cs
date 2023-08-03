using FluentMigrator;
using FluentMigrator.Runner;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;
using Serilog;
using Serilog.Events;

namespace Database.Migrations;

internal static class Program
{
    private static IConfiguration? _configuration;

    private static IConfiguration Configuration
    {
        get
        {
            if (_configuration != null) return _configuration;

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets(typeof(Program).Assembly)
                .AddEnvironmentVariables()
                .Build();
            _configuration = configurationRoot;

            return configurationRoot;
        }
    }

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
        Log.Information("Starting db migrator");
        try
        {
            using var serviceProvider = CreateServices();
            using var scope = serviceProvider.CreateScope();

            var migrationDirection = args.Length > 0
                ? Enum.Parse<MigrationDirection>(args[0], true)
                : MigrationDirection.Up;

            var retryPolicy = Policy
                .Handle<NpgsqlException>()
                .WaitAndRetry(5, i =>
                {
                    var sleepDuration = i * 3.Seconds();
                    Log.Warning("Wait for {Duration} and retry", sleepDuration.Humanize());
                    return sleepDuration;
                });
            retryPolicy.Execute(() =>
            {
                try
                {
                    UpdateDatabase(scope.ServiceProvider, migrationDirection);
                }
                catch (NpgsqlException exception)
                {
                    Log.Error(exception, "Failed to update database");
                    if (exception is { IsTransient: false, InnerException: not null }) throw exception.InnerException;

                    throw;
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "General exception");
                }
            });
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Db migrator terminated unexpectedly");
        }
        finally
        {
            Log.Information("Db migrator shutting down...");
            Log.CloseAndFlush();
        }
    }

    private static ServiceProvider CreateServices()
    {
        return new ServiceCollection()
            .AddScoped(_ => Configuration)
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(provider =>
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    var connectionString = configuration.GetConnectionString("migration-db");
                    Log.Debug("ConnectionString: {ConnectionString}", connectionString);
                    return connectionString;
                })
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    private static void UpdateDatabase(IServiceProvider scopeServiceProvider, MigrationDirection migrationDirection)
    {
        var migrationRunner = scopeServiceProvider.GetRequiredService<IMigrationRunner>();
        if (Log.IsEnabled(LogEventLevel.Debug))
        {
            Log.Debug("List all migrations");
            migrationRunner.ListMigrations();
        }

        if (migrationDirection == MigrationDirection.Up)
        {
            if (!migrationRunner.HasMigrationsToApplyUp())
            {
                Log.Warning("No applicable migrations to apply up");
                return;
            }

            Log.Information("Execute all found (and not applied) migrations");

            migrationRunner.ValidateVersionOrder();
            migrationRunner.MigrateUp();
        }
        else
        {
            if (!migrationRunner.HasMigrationsToApplyRollback())
            {
                Log.Warning("No applicable migrations to apply rollback");
                return;
            }

            Log.Information("Rollback one step");
            migrationRunner.Rollback(1);
        }
    }
}
