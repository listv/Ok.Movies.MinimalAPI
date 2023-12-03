using FluentValidation;
using Infrastructure.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<DatabaseOptions>, DatabaseOptionsValidator>();

        services.AddOptions<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.SectionName)
            .ValidateFluently()
            .ValidateOnStart();

        var serviceProvider = services.BuildServiceProvider();
        var serviceProviderFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceProviderFactory.CreateScope();
        var databaseSettings = serviceScope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(databaseSettings.ConnectionString));
        
        return services;
    }
}
