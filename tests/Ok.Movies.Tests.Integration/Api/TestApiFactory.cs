using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace Ok.Movies.Tests.Integration.Api;

public class TestApiFactory : WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders());
        builder.UseSetting("integrationTest", "true");
    }
}
