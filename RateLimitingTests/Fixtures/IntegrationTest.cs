using Microsoft.Extensions.Configuration;
using Respawn;
using System.Net.Http;
using Xunit;

namespace RateLimitingTests.Fixtures
{
    public abstract class IntegrationTest : IClassFixture<RateLimitingWebApplicationFactory>
    {
        //private readonly Checkpoint _checkpoint = new Checkpoint
        //{
        //    SchemasToInclude = new[] {
        //    "Playground"
        //},
        //    WithReseed = true
        //};

        protected readonly RateLimitingWebApplicationFactory _factory;
        protected readonly HttpClient _client;
        protected readonly IConfiguration _configuration;

        public IntegrationTest(RateLimitingWebApplicationFactory fixture)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
            _configuration = new ConfigurationBuilder()
                  .Build();

            // if needed, reset the DB
            //_checkpoint.Reset(_configuration.GetConnectionString("SQL")).Wait();
        }
    }

}