using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RateLimiting.Models;
using RateLimiting.Security.Services;
using RateLimiting.Security.Auth.BasicAuth;
using RateLimiting.Security.Auth.JwtAuth;
using RateLimiting.Models.Random;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("[controller]")]   
    public class RandomController : ControllerBase
    {
        // private readonly RateLimitingContext _context;
        private IUserService _userService;
        private IRateLimitService _rateLimitService;
        public IRateLimitService RateLimitService {
            get{
                return _rateLimitService;
            }
            set {}
        }
        
        public RandomController(IUserService userService, IRateLimitService rateLimitService)
        {
            _userService = userService;
            _rateLimitService = rateLimitService;
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
        public async Task<IActionResult> GetRandom(long len = 32)
        {
            await Task.Delay(100);
            return Ok(new { message = "Requested rate spent :)", random = new Item(len).random});
        }

    }
}
