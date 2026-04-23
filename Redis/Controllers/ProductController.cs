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
        [Route("GetAllProducts")]
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

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
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
        [Route("CreateProduct")]
        public async Task<IActionResult> Post(Product product)
        {
            var data = await _service.Create(product);

            return data == null ? NotFound(new
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Data Not Found"
            }) :
            Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Product Create Successfully",
                Data = data
            });
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Put(Product product)
        {
            var result = await _service.Update(product);

            return result == true ? Ok(new
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Data Updated Successfully"
            }) : NotFound(new
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = "Data Not Found"
            });
        }

        [HttpDelete("DeleteProduct/{id}")]
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
