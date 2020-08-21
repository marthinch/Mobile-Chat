using Matcha.BackgroundService;
using MobileChat.Models;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileChat.BackgroundService
{
    public class PeriodicTask : IPeriodicTask
    {
        public PeriodicTask(int interval)
        {
            ChatNotification();

            Interval = TimeSpan.FromSeconds(interval);
        }

        public TimeSpan Interval { get; set; }

        public Task<bool> StartJob()
        {
            return Task.FromResult(true);
        }

        private async void ChatNotification()
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await clientWebSocket.ConnectAsync(new Uri("ws://192.168.0.101:3000"), cancellationTokenSource.Token);

                await Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        WebSocketReceiveResult result;
                        var arraySegment = new ArraySegment<byte>(new byte[4096]);

                        do
                        {
                            result = await clientWebSocket.ReceiveAsync(arraySegment, cancellationTokenSource.Token);
                            var messageBytes = arraySegment.Skip(arraySegment.Offset).Take(result.Count).ToArray();
                            string serialisedMessage = Encoding.UTF8.GetString(messageBytes);

                            try
                            {
                                var message = JsonConvert.DeserializeObject<Message>(serialisedMessage);
                                if (message != null && message.IsIncoming)
                                {
                                    NotificationRequest notification = new NotificationRequest
                                    {
                                        NotificationId = 100,
                                        Title = "New message",
                                        Description = "You have new messages",
                                        ReturningData = serialisedMessage
                                    };
                                    NotificationCenter.Current.Show(notification);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Invalide message format. {ex.Message}");
                            }

                        } while (!result.EndOfMessage);
                    }
                }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
