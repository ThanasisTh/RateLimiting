using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using RateLimiting.Models.Authentication;
using RateLimiting.Security.Entities;
using RateLimiting.Models.Configuration;
using RateLimiting.Data;

namespace RateLimiting.Security.Services
{
    /// <summary>
    /// The service that takes care of handling <see cref="Security.Entities.User" /> related business logic. 
    /// </summary>
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        User GetById(int id);
        bool isValidUser(string username, string password);
        int consumeBandwidth(int Id, int bytes);
        void changeLimit(int id, int limit);
    }

    /// <summary>
    /// The service that takes care of handling <see cref="Security.Entities.User" /> related business logic. 
    /// </summary>
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private RateLimitingContext _rateLimitingContext;

        /// <summary>
        /// The service that takes care of handling <see cref="Security.Entities.User" /> related business logic. 
        /// </summary>
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
            User user = (User) _rateLimitingContext.Users.Find(id);
            
            if (user == null) return -1;

            if (user.Bandwidth >= bytes) {
                user.Bandwidth -= bytes;
                _rateLimitingContext.Users.Update(user);
                _rateLimitingContext.SaveChangesAsync();
                return user.Bandwidth;
            };
            return -1;
        }

        public void changeLimit(int id, int limit) {
            User user = (User) _rateLimitingContext.Users.Find(id);
            
            if (user == null) return;

            user._limit = limit;
            _rateLimitingContext.Users.Update(user);
            
            _rateLimitingContext.SaveChangesAsync();
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