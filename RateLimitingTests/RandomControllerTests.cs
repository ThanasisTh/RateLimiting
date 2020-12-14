using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Text;
using RateLimiting.Security.Entities;
using RateLimitingTests.Fixtures;

namespace RateLimitingTests
{
    public class RandomControllerTests: IntegrationTest {
        public HttpClient _client {get;}

        public RandomControllerTests(RateLimitingWebApplicationFactory fixture) :base(fixture) {
            
        }

        [Fact]
        public async Task UserShouldRegister()
        {
            var payload = new User{
                FirstName = "first",
                LastName = "last",
                Username = "test",
                Password = "test"
            };
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/random/register", httpContent);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
    
            var userDTO = JsonConvert.DeserializeObject<UserDTO[]>(await response.Content.ReadAsStringAsync());
            userDTO.GetType().GetProperty("Id").Should().Equals(2);
        }
    }
}