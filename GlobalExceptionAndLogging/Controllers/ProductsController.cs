using GlobalExceptionAndLogging.Entities;
using GlobalExceptionAndLogging.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionAndLogging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _service.GetByIdAsync(id);

            return Ok(product);
        }
    }
}
