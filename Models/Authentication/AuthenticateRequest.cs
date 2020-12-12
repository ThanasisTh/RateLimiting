using System.ComponentModel.DataAnnotations;

namespace RateLimiting.Models.Authentication
{
    public class AuthenticateRequest
    {
        public AuthenticateRequest(string username, string password) {
            Username = username;
            Password = password;
        }
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}