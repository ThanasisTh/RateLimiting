using Microsoft.EntityFrameworkCore;
using RateLimiting.Models.Random;
using RateLimiting.Security.Entities;

namespace RateLimiting.Data {
    /// <summary>
    /// The <see cref="DbContext"/> used to store and access instances of
    /// <see cref="RateLimiting.Security.Entities.User"/> registered through "/register" 
    /// </summary>
    public class RateLimitingContext : DbContext
    {
        private string _connection;
        public RateLimitingContext (DbContextOptions<RateLimitingContext> options)
            : base(options)
        {
        }

        public RateLimitingContext (DbContextOptions<RateLimitingContext> options, string connection)
        {
            _connection = connection;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // doesn't seem to work properly because bandwidth is non-nullable type int, focusing on functionality rather than resolve this 
            modelBuilder.Entity<User>().Property(p => p.Bandwidth).HasDefaultValueSql("1024");
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (_connection != null)
            {
                var config = _connection;
                optionsBuilder.UseInMemoryDatabase(config);
            }

            base.OnConfiguring(optionsBuilder);
        }
        
        public DbSet<User> Users {get; set;}
    }
}