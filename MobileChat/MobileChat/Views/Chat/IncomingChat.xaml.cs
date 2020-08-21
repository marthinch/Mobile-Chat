
using MobileChat.Models;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingChat : ViewCell
    {
        public IncomingChat()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var currentBindingContext = BindingContext as Message;
            if (currentBindingContext != null)
            {
                var imageSource = ImageSource.FromStream(() => new MemoryStream(currentBindingContext.FileBytes));
                ImageChat.Source = imageSource;
            }
        }
    }
}