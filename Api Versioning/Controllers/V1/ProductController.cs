using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Api_Versioning.Controllers.V1
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
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