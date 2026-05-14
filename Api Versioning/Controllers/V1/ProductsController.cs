using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Api_Versioning.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("GelAll")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "V1",
                Data = "Old Response"
            });
        }
    }
}