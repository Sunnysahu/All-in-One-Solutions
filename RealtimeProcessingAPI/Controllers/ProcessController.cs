using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealtimeProcessingAPI.Interfaces;
using System.Text.Json;

namespace RealtimeProcessingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessingService _service;

        public ProcessController(IProcessingService service) => _service = service;


        [HttpGet("stream")]
        public async Task Stream(CancellationToken cancellationToken)
        {
            Response.StatusCode = StatusCodes.Status200OK;

            Response.ContentType = "text/event-stream";

            Response.Headers.Append("Cache-Control","no-cache");

            Response.Headers.Append("Connection", "keep-alive");

            await Response.StartAsync(cancellationToken);

            await foreach (var item in _service.ProcessAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var json =
                    JsonSerializer.Serialize(item);

                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);

                await Response.Body.FlushAsync(cancellationToken);
            }
        }
    }
}
