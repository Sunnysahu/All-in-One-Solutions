using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Versioning.Controllers
{
    [ApiController]
    [ApiVersion("1.0")] // Declares what versions a controller/action supports
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")] // fallback route (important)
    public class SameController : ControllerBase
    {
        [MapToApiVersion("1.0")] // Specifies which version a specific action belongs to
        [HttpGet("GetAll")]
        public IActionResult GetV1()
        {
            return Ok(new
            {
                Version = "v1",
                Data = "Old",
                F_Name = "Sunny",
                L_Name = "Sahu"
            });
        }

        [MapToApiVersion("2.0")]
        [HttpGet("GetAll")]
        public IActionResult GetV2()
        {
            return Ok(new
            {
                Version = "v2",
                Data = "New",
                FullName = "Sunny Sahu"
            });
        }

        /*
         What happens without MapToApiVersion?

        If you only Do

        [ApiVersion("1.0")]
        [ApiVersion("2.0")]

        Then --> ASP.NET tries to match actions automatically and If multiple actions match → ambiguity error

        [HttpGet("GetAll")]
        public IActionResult GetV1()

        [HttpGet("GetAll")]
        public IActionResult GetV2()

        ERROR: Ambiguous match
         */
    }
}