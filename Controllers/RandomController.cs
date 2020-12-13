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


        [AllowAnonymous]
        [HttpPostAttribute("register")]
        public async Task<ActionResult<User>> registerUser(User user) {
            user.Bandwidth = 1024;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new CreatedResult("", new UserDTO(user));
        }

        // basic authentication login, requires username + password, generates JWT which lasts 5 minutes
        [BasicAuth]
        [HttpPost("authenticate")]
        public IActionResult Authenticate()
        {
            return Ok();
        }

        // bearer token authentication, requires generated JWT
        // GET: /random
        [JwtAuth]
        [HttpGet]
        public async Task<IActionResult> GetRandom(long len = 32)
        {
            await Task.Delay(100);
            return Ok(new { message = "Requested rate spent :)", random = new Item(len).random});
        }
    }
}
