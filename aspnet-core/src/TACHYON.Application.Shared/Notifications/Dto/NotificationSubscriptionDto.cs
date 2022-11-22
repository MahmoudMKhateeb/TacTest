using Abp.Notifications;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Notifications.Dto
{
    public class NotificationSubscriptionDto
    {
        [Required]
        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public string Name { get; set; }

        public bool IsSubscribed { get; set; }
        
        public bool IsEmailSubscribed { get; set; }
    }
}