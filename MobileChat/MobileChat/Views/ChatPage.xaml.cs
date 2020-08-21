using MobileChat.Models;
using MobileChat.ViewModels;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        ChatViewModel chatViewModel;

        public ChatPage(string userName)
        {
            InitializeComponent();

            chatViewModel = new ChatViewModel(userName);

            BindingContext = chatViewModel;

            ((INotifyCollectionChanged)MessagesListView.ItemsSource).CollectionChanged += ChatPage_CollectionChanged;
        }

        public ChatPage(Message message)
        {
            InitializeComponent();

            chatViewModel = new ChatViewModel();

            BindingContext = chatViewModel;

            if (message != null)
                chatViewModel.Messages.Add(message);

            ((INotifyCollectionChanged)MessagesListView.ItemsSource).CollectionChanged += ChatPage_CollectionChanged;
        }

        private void ChatPage_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (chatViewModel.Messages.Count > 0)
            {
                var message = chatViewModel.Messages[chatViewModel.Messages.Count - 1];

                Device.BeginInvokeOnMainThread(() =>
                {
                    MessagesListView.ScrollTo(message, ScrollToPosition.End, true);
                });
            }
        }
    }
}