using Microsoft.EntityFrameworkCore;

namespace RateLimiting.Models
{
    public class RateLimitingItemContext : DbContext
    {
        public RateLimitingItemContext(DbContextOptions<RateLimitingItemContext> options)
            : base(options)
        {
        }

        public DbSet<Item>  RateLimitingItems { get; set; }
    }
}