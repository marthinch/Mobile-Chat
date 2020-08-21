using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Services
{
    public class ChatService
    {
        readonly RequestDelegate requestDelegate;
        ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();

        public ChatService(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await requestDelegate.Invoke(context);
                return;
            }

            var token = context.RequestAborted;
            var socket = await context.WebSockets.AcceptWebSocketAsync();

            var guid = Guid.NewGuid().ToString();
            sockets.TryAdd(guid, socket);

            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                var message = await GetMessageAsync(socket, token);
                System.Console.WriteLine($"Received message - {message} at {DateTime.Now}");

                if (string.IsNullOrEmpty(message))
                {
                    if (socket.State != WebSocketState.Open)
                        break;

                    continue;
                }

                foreach (var s in sockets.Where(p => p.Value.State == WebSocketState.Open))
                    await SendMessageAsync(s.Value, message, token);
            }

            sockets.TryRemove(guid, out WebSocket redundantSocket);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended", token);
            socket.Dispose();
        }

        async Task<string> GetMessageAsync(WebSocket socket, CancellationToken token)
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            string receivedMessage = string.Empty;

            do
            {
                token.ThrowIfCancellationRequested();

                result = await socket.ReceiveAsync(message, token);
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage = Encoding.UTF8.GetString(messageBytes);

            } while (!result.EndOfMessage);

            if (result.MessageType != WebSocketMessageType.Text)
                return null;

            return receivedMessage;
        }

        Task SendMessageAsync(WebSocket socket, string message, CancellationToken token)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(byteMessage);

            return socket.SendAsync(segment, WebSocketMessageType.Text, true, token);
        }
    }
}
