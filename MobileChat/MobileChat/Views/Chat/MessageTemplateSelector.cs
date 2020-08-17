using MobileChat.Models;
using Xamarin.Forms;

namespace MobileChat.Views.Chat
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate incomingChatTemplate;
        private readonly DataTemplate outgoingChatTemplate;

        public MessageTemplateSelector()
        {
            this.incomingChatTemplate = new DataTemplate(typeof(IncomingChat));
            this.outgoingChatTemplate = new DataTemplate(typeof(OutgoingChat));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as Message;
            if (message == null)
                return null;

            return message.IsIncoming ? this.incomingChatTemplate : this.outgoingChatTemplate;
        }
    }
}
