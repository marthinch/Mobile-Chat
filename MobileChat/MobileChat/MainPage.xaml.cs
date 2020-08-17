using MobileChat.ViewModels;
using Xamarin.Forms;

namespace MobileChat
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel();
        }
    }
}
