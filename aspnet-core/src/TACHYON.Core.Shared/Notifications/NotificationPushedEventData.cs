using Abp;
using Abp.Events.Bus;

namespace TACHYON.Notifications
{
    public class NotificationPushedEventData : EventData
    {
        public UserIdentifier[] UserIdentifiers { get; set; }

        public string Name { get; set; }
        
        public string DisplayName { get; set; }

        public string Message { get; set; }
    }
}