using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RateLimiting.Models;
using RateLimiting.Security.Services;
using RateLimiting.Security.Auth.BasicAuth;
using RateLimiting.Security.Auth.JwtAuth;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("[controller]")]   
    public class RandomController : ControllerBase
    {
        // private readonly RateLimitingContext _context;
        private IUserService _userService;
        
        public RandomController(IUserService userService)
        {
            _userService = userService;
        }

        [BasicAuth]
        [HttpPost("authenticate")]
        public IActionResult Authenticate()
        {
            return Ok();
        }

        // GET: /random
        [JwtAuth]
        [HttpGet]
        public async Task<IActionResult> GetRandom()
        {
            await Task.Delay(100);
            return Ok(new Item());
        }

        // GET: /random/5
        [JwtAuth]
        [HttpGet("{len}")]
        public async Task<IActionResult> GetRandom(long len)
        {
            await Task.Delay(100);
            return Ok(new Item(len));
        }

    }
}
