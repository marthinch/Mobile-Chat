using MobileChat.Views;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileChat.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public string Username { get { return GetValue<string>(); } set { SetValue(value); } }

        public ICommand SignUpCommand
        {
            get
            {
                return new Command(async () => await SignUp());
            }
        }

        private async Task SignUp()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ChatPage(Username), true);
        }
    }
}
