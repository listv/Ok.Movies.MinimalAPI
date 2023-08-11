namespace Api.Configurations;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        const string configurationsDirectory = "Configurations";
        var environmentName = builder.Environment.EnvironmentName;
        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddJsonFile($"{configurationsDirectory}/logging.json", false, true)
            .AddJsonFile($"{configurationsDirectory}/logging.{environmentName}.json", true, true)
            .AddUserSecrets(typeof(IApiMarker).Assembly)
            .AddEnvironmentVariables();

        return builder;
    }
}
