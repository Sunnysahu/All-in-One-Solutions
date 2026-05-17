using System.Net.WebSockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Web_Socket.Services
{
    public class WebSocketRepository : IWebSocketRepository
    {
        public async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4]; // WebSocket receives raw bytes. Buffer means temporary memory storage.

            try
            {
                while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested) // WebSocket stays persistent, continuously listening for messages until the connection closes.
                {
                    // This waits for client message.
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken
                    );

                    if (result.MessageType == WebSocketMessageType.Close) // Client may disconnect.
                    {
                        // If close request comes
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Connection Closed",
                            cancellationToken
                        );

                        break;
                    }

                    // Network data comes as bytes. Need conversion to readable text
                    var clientMessage = Encoding.UTF8.GetString(buffer, 0, result.Count); // Buffer size is 4096, but the actual message may be only 20 bytes, so read only the actual message length.

                    Console.WriteLine($"Client Message: {clientMessage}");

                    var serverMessage = $"Server Received: {clientMessage}";

                    var serverBytes = Encoding.UTF8.GetBytes(serverMessage);

                    // Sends message back to client.
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(serverBytes),
                        WebSocketMessageType.Text,
                        true, // This is complete message and Called EndOfMessage.
                        cancellationToken
                    );
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("WebSocket timeout / cancelled");
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Server Closing Connection",
                        cancellationToken
                    );
                }
                webSocket.Dispose(); // Release resources associated with the WebSocket connection.
            }

        }
    }
}
