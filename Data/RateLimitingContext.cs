using Microsoft.EntityFrameworkCore;
using RateLimiting.Models;

    public class RateLimitingContext : DbContext
    {
        public RateLimitingContext (DbContextOptions<RateLimitingContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; }
    }
