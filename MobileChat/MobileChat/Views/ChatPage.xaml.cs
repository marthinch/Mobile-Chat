using MobileChat.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage(string userName)
        {
            InitializeComponent();

            BindingContext = new ChatViewModel(userName);
        }
    }
}