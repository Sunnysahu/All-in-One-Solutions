using System.Net.WebSockets;

namespace Web_Socket.Services
{
    public interface IWebSocketRepository
    {
        Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken);
    }
}
