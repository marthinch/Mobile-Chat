using MobileChat.Models;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileChat.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private ClientWebSocket clientWebSocket;
        private CancellationTokenSource cancellationTokenSource;
        private string userName;


        public ObservableCollection<Message> Messages { get { return GetValue<ObservableCollection<Message>>(); } set { SetValue<ObservableCollection<Message>>(value); } }
        public string MessageText { get { return GetValue<string>(); } set { SetValue<string>(value); } }


        public ICommand SendCommand
        {
            get
            {
                return new Command(async () => await Send());
            }
        }


        public ChatViewModel(string userName)
        {
            this.userName = userName;

            clientWebSocket = new ClientWebSocket();
            cancellationTokenSource = new CancellationTokenSource();

            Messages = new ObservableCollection<Message>();

            ConnectToServerAsync();
        }

        private async void ConnectToServerAsync()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await clientWebSocket.ConnectAsync(new Uri("ws://localhost:5000"), cancellationTokenSource.Token);
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    //await clientWebSocket.ConnectAsync(new Uri("ws://10.0.2.2:5000"), cancellationTokenSource.Token);
                    await clientWebSocket.ConnectAsync(new Uri("ws://192.168.0.101:3000"), cancellationTokenSource.Token);
                }

                UpdateClientState();

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
                            string serialisedMessae = Encoding.UTF8.GetString(messageBytes);

                            try
                            {
                                var message = JsonConvert.DeserializeObject<Message>(serialisedMessae);
                                Messages.Add(message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Invalide message format. {ex.Message}");
                            }

                        } while (!result.EndOfMessage);
                    }
                }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                UpdateClientState();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void UpdateClientState()
        {
            Console.WriteLine($"Websocket state {clientWebSocket.State}");
        }

        private async Task Send()
        {
            try
            {
                Message message = new Message
                {
                    Name = userName,
                    MessagDateTime = DateTime.Now,
                    Text = MessageText,
                    UserId = CrossDeviceInfo.Current.Id
                };

                string serialisedMessage = JsonConvert.SerializeObject(message);

                var byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
                var segmnet = new ArraySegment<byte>(byteMessage);

                await clientWebSocket.SendAsync(segmnet, WebSocketMessageType.Text, true, cancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            MessageText = string.Empty;
        }
    }
}
