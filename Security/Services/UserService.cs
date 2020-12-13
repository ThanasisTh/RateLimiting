using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using RateLimiting.Data;
using System.Security.Claims;
using System.Text;
using RateLimiting.Models.Authentication;
using RateLimiting.Security.Entities;
using RateLimiting.Models.Configuration;

namespace RateLimiting.Security.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        User GetById(int id);
        bool isValidUser(string username, string password);
        int consumeBandwidth(int Id, int bytes);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        // private List<User> _users = new List<User>
        // {
        //     new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Bandwidth = 1024, Password = "test" }
        // };
        private int _limit = 1024;
        private List<User> _users = new List<User>{};

        private readonly AppSettings _appSettings;
        private RateLimitingContext _rateLimitingContext;

        public UserService(IOptions<AppSettings> appSettings, RateLimitingContext rateLimitingContext)
        {
            _appSettings = appSettings.Value;
            _rateLimitingContext = rateLimitingContext;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _rateLimitingContext.Users.ToList().FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public User GetById(int id)
        {
            return _rateLimitingContext.Users.Find(id);
        }

        public int consumeBandwidth(int id, int bytes) {
            var user = _rateLimitingContext.Users.Find(id);
            if (user.Bandwidth >= bytes) {
                user.Bandwidth -= bytes;
                _rateLimitingContext.Users.Update(user);
                _rateLimitingContext.SaveChangesAsync();
                return user.Bandwidth;
            };
            return -1;
        }

        public int reportBandwidth(User user)
        {
            return user.Bandwidth;
        }

        private int resetBandwidth(User user) {
            user.Bandwidth = _limit;
            return user.Bandwidth;
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool isValidUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            return true;
        }
    }
}