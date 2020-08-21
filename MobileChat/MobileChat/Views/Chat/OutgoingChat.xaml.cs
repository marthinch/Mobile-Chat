
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutgoingChat : ViewCell
    {
        public OutgoingChat()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }
    }
}