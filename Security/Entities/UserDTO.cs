using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RateLimiting.Security.Entities
{
    // DTO response object that omits user password 
    // (avoiding [JsonIgnore] in User class for simplicity of instatiating db objects)
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public UserDTO(User user) {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
        }
    }
}