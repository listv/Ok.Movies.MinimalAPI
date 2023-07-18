﻿namespace Api.Configurations;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        const string configurationsDirectory = "Configurations";
        var environmentName = builder.Environment.EnvironmentName;
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/logging.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/logging.{environmentName}.json", optional: true, reloadOnChange: true);
        
        return builder;
    }
}
