using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RateLimiting.Data;
using RateLimiting.Security.Services;
using RateLimiting.Security.Auth.BasicAuth;
using RateLimiting.Security.Auth.JwtAuth;
using RateLimiting.Models.Random;
using Microsoft.AspNetCore.Authorization;
using RateLimiting.Security.Entities;
using System;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("[controller]")]   
    public class RandomController : ControllerBase
    {
        // private readonly RateLimitingContext _context;
        private IUserService _userService;
        private RateLimitingContext _context;
        
        public RandomController(RateLimitingContext context, IUserService userService)
        {
            _userService = userService;
            _context = context;
            DateTime resetTime = DateTime.UtcNow.AddSeconds(100);
        }

        /// <summary>
        /// Initial request, register a <see cref="RateLimiting.Security.Entities.User"/> to the DB. 
        /// </summary>
        /// <param name="user">A <see cref="RateLimiting.Security.Entities.User"/> instance</param>
        [AllowAnonymous]
        [HttpPostAttribute("register")]
        public async Task<ActionResult<User>> registerUser(User user) {
            user.Bandwidth = 1024;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new CreatedResult("", new UserDTO(user));
        }

        /// <summary>
        /// Basic authentication login, requires Base64 encoded "username:password", generates JWT which lasts 5 minutes
        /// </summary>
        /// <returns><para>An <see cref="Models.Authentication.AuthenticateResponse" /> instance, 
        ///          containing a JWT token used to access "/random". </para>
        ///          <para>The JWT expires after 5 minuts. </para>
        /// </returns>
        [BasicAuth]
        [HttpPost("authenticate")]
        public IActionResult Authenticate()
        {
            return Ok();
        }

        /// <summary>
        /// Bearer token authentication, requires generated JWT
        /// </summary>
        /// <param name="len">The length of the requested randomness</param>
        /// <returns>An object containing a message and (if the request was successful)
        ///          a random Base64 encoded string of the requested length.
        /// </returns>
        [JwtAuth]
        [HttpGet]
        public async Task<IActionResult> GetRandom(long len = 32)
        {
            await Task.Delay(100);
            return Ok(new { message = "Requested rate spent :)", random = new Item(len).random});
        }
    }
}
