using Plugin.DeviceInfo;
using System;

namespace MobileChat.Models
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime MessagDateTime { get; set; }

        public bool IsIncoming => UserId != CrossDeviceInfo.Current.Id;

        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
