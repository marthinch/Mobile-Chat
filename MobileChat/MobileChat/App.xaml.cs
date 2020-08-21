using Matcha.BackgroundService;
using MobileChat.BackgroundService;
using MobileChat.Models;
using MobileChat.Views;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Xamarin.Forms;

namespace MobileChat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            // Local Notification tap event listener
            NotificationCenter.Current.NotificationTapped += Current_NotificationTapped;
        }

        private async void Current_NotificationTapped(NotificationTappedEventArgs e)
        {
            var data = e.Data;
            if (!string.IsNullOrEmpty(data))
            {
                var message = JsonConvert.DeserializeObject<Message>(data);
                await App.Current.MainPage.Navigation.PushAsync(new ChatPage(message), true);
            }
        }

        protected override void OnStart()
        {
            // Running the periodic task in the background
            BackgroundAggregatorService.Add(() => new PeriodicTask(10));
            BackgroundAggregatorService.StartBackgroundService();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {

        }
    }
}
