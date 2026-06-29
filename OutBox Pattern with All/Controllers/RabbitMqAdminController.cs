using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutBox_Pattern_with_All.Models;
using OutBox_Pattern_with_All.Services;

namespace OutBox_Pattern_with_All.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMqAdminController : ControllerBase
    {
        private readonly RabbitMqManagementService _service;

        public RabbitMqAdminController(RabbitMqManagementService service) => _service = service;


        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            List<QueueInfo>? queues = await _service.GetQueuesAsync();

            return Ok(queues);
        }
    }
}
