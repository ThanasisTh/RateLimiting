using Microsoft.EntityFrameworkCore;
using RateLimiting.Models.Random;
using RateLimiting.Security.Entities;

namespace RateLimiting.Data {
    public class RateLimitingContext : DbContext
    {
        public RateLimitingContext (DbContextOptions<RateLimitingContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; }
        public DbSet<User> User {get; set;}
    }
}