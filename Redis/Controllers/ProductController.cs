using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Redis.Models;
using Redis.Services;

namespace Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {

            var data = await _service.GetAll();
            
            return data == null ? NotFound(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Data Not Found"
            }) :
            Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Data Feteched Successfully",
                Data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _service.GetById(id);

            return data == null ? NotFound(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Data Not Found"
            }) :
            Ok(new
            { 
                StatusCode = StatusCodes.Status200OK,
                Message = "Data Feteched Successfully",
                Data = data 
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            var data = await _service.Create(product);

            return data == null ? NotFound(new
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Data Not Found"
            }) :
            CreatedAtAction(nameof(Get), new { id = data.Id }, new
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Product created successfully",
                Data = data
            });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Product product)
        {
            var result = await _service.Update(product);

            return result == true ? Ok(new
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Data Feteched Successfully"
            }) : NotFound(new
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Data Not Found"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Data Deleted Successfully"
            });
        }
    }
}
