using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RateLimiting.Data;
using RateLimiting.Security.Entities;

namespace RateLimiting.Security.Services
{
    /// <summary>
    /// The daemon-like <cref="IHostedService"/> that takes care of resetting the bandwidth for each client every 10 seconds
    /// </summary>
    public class RateLimitService : IHostedService, IDisposable {
        private RateLimitingContext _rateLimitingContext; 
        private Timer _timer;
        
        /// <summary>
        /// The daemon-like <cref="IHostedService"/> that takes care of resetting the bandwidth for each client every 10 seconds
        /// </summary>
        public RateLimitService(RateLimitingContext rateLimitingContext) {
            _rateLimitingContext = rateLimitingContext;
            addAdminToDb();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new System.Threading.Timer((e) => {
                this.resetBandwidth();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void addAdminToDb() {
            User admin = new User();
            admin.Id = 1;
            admin.Username = "admin";
            admin.Password = "admin";
            _rateLimitingContext.Add(admin);
            _rateLimitingContext.SaveChangesAsync();
        }
        
        private async void resetBandwidth() {
            Console.WriteLine("resetting limit period...");
            var users = _rateLimitingContext.Users.ToList();

            if (users == null) return;
            
            foreach (User user in users) {
                user.Bandwidth = user._limit;
                _rateLimitingContext.Users.Update(user);
            }
            await _rateLimitingContext.SaveChangesAsync();
        }
    }
}