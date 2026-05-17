using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Web_Socket.Services;

namespace Web_Socket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly IWebSocketRepository _repository;

        public WebSocketController(IWebSocketRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("connect")]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest) // Did client request WebSocket upgrade?
            {
                var webSocketOptions = new WebSocketAcceptContext
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(30),
                    SubProtocol = "chat"
                };

                
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(webSocketOptions);

                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

                await _repository.HandleWebSocketAsync(webSocket, cts.Token);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
