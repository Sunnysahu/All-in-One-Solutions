using Dependency_Injection_with_DB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dependency_Injection_with_DB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _service.GetProductsAsync();
            return Ok(data);
        }
    }
}
