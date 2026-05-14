using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Api_Versioning.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("GelAll")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "V2",
                Data = "New Response",
                ExtraField = "Added in V2"
            });
        }
    }

}