using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiting.Security.Entities;

namespace RateLimitingTests.Fixtures
{
    public class RateLimitingWebApplicationFactory : WebApplicationFactory<RateLimiting.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                var integrationConfig = new ConfigurationBuilder()
                  .Build();

                config.AddConfiguration(integrationConfig);
            });

            // is called after the `ConfigureServices` from the Startup
            builder.ConfigureTestServices(services =>
            {
                // services.AddTransient<IWeatherForecastConfigService, WeatherForecastConfigStub>();
            });
        }
    }
}