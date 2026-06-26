using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlobalAnnotationUtils.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateLimitingController : ControllerBase
    {
        // Fixed Window Limiter

        // 1-5 requests → 200
        // 6th request → 429
        [EnableRateLimiting("fixed")]
        [Route("fixed")]
        [HttpGet]
        public IActionResult Fixed()
        {
            return Ok("Fixed Success");
        }

        [EnableRateLimiting("sliding")]
        [Route("sliding")]
        [HttpGet]
        public IActionResult Sliding()
        {
            return Ok("Sliding Success");
        }

        [EnableRateLimiting("token")] // pass X-User in header part in postman
        [Route("token")]
        [HttpGet]
        public IActionResult Token()
        {
            return Ok("Token Success");
        }

        [EnableRateLimiting("user-policy")]
        [Route("user-policy")]
        [HttpGet]
        public IActionResult UserPolicy()
        {
            return Ok("Token Success");
        }
        
        [EnableRateLimiting("ip-policy")]
        [Route("ip-policy")]
        [HttpGet]
        public IActionResult IpPolicy()
        {
            var ip = HttpContext.Connection.RemoteIpAddress;
            Console.WriteLine(ip);
            return Ok("Token Success");
        }


        [EnableRateLimiting("concurrent")]
        [Route("concurrent")]
        [HttpGet]
        public async Task<IActionResult> Concurrent()
        {
            await Task.Delay(5000);
            return Ok("Concurrent Success");
        }
    }
}
