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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(p => p.Bandwidth).HasDefaultValueSql("1024");
        } 
        
        public DbSet<User> Users {get; set;}
    }
}