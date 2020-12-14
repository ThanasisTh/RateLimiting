using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RateLimiting.Security.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Bandwidth { get; set; } = 1024;
        // get only used by reset, set only used for admin changing  
        public int _limit {get; set;} = 1024;
    }
}