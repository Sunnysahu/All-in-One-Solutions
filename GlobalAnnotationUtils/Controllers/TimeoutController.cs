using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace GlobalAnnotationUtils.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeoutController : ControllerBase
    {
        [HttpGet("timeout")]
        //[RequestTimeout("Timeout")] // 5 Second Timeout
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            return Ok("Completed");
        }

        [HttpGet("short")]
        [RequestTimeout("ShortTimeout")]
        public async Task<IActionResult> Short(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            return Ok("Done");
        }

        [HttpGet("simpleTimeout")]
        [RequestTimeout(2000)]
        public async Task<IActionResult> simpleTimeout(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            return Ok();
        }

    }
}
