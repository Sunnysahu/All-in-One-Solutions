using Microsoft.AspNetCore.DataProtection;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace Web_Socket.Services
{
    public class WebSocketRepository : IWebSocketRepository
    {
        public async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4]; // WebSocket receives raw bytes. Buffer means temporary memory storage.

            try
            {

                // if you want to send something to client when connection established, you can do it here. But this is not necessary. WebSocket is persistent connection, so you can send message to client whenever you want until connection closes.

                //for (int i = 1; i <= 20; i++)
                //{
                //    var welcomeMessage = Encoding.UTF8.GetBytes($"Welcome Client {i}");

                //    await Task.Delay(500, cancellationToken);

                //    await webSocket.SendAsync(
                //        new ArraySegment<byte>(welcomeMessage),
                //        WebSocketMessageType.Text,
                //        true,
                //        CancellationToken.None
                //    );
                //}

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

        public async Task HandleAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            try
            {
                while (IsSocketOpen(webSocket, cancellationToken))
                {
                    var message = await ReceiveAsync(webSocket, cancellationToken, buffer);

                    if (message.Type == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closed by client",
                            cancellationToken
                        );

                        break;
                    }

                    Console.WriteLine($"Client: {message.Text}");

                    for (int i = 1; i <= (int.TryParse(message.Text, out var num) ? num : message.Text.Length); i++)
                    {
                        if (!IsSocketOpen(webSocket, cancellationToken)) break;

                        await SendAsync(
                        webSocket,
                        $"Server Received: {i} of {message.Text} ",
                        cancellationToken
                        );

                        await Task.Delay(500, cancellationToken);
                    }

                    await SendAsync(
                        webSocket,
                        $"Server Received: {message.Text}",
                        cancellationToken
                    );
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("WebSocket cancelled safely"); // THIS IS NORMAL (Postman timeout / server shutdown)
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message} // Connection Timeout");
            }
            finally
            {
                await SafeCloseAsync(webSocket, cancellationToken);
            }
        }

        private bool IsSocketOpen(WebSocket socket, CancellationToken token) => 
            !token.IsCancellationRequested && socket.State == WebSocketState.Open;

        private async Task<(string Text, WebSocketMessageType Type)> ReceiveAsync(WebSocket socket, CancellationToken token, byte[] buffer)
        {
            var result = await socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                token
            );

            var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
            return (text, result.MessageType);
        }

        private Task SendAsync(WebSocket socket, string message, CancellationToken token)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            return socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                token
            );
        }

        private async Task SafeCloseAsync(WebSocket socket, CancellationToken token)
        {
            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Server shutdown",
                    token
                );
            }
        }
    }
}