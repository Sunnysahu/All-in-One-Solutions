using GlobalAnnotationUtils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlobalAnnotationUtils.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(
                    result.Error!.StatusCode,
                    result.Error);
            }

            return Ok(new
            {
                result.Error!.StatusCode,
                result.Value
            });
        }
    }
}
