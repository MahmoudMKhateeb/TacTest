using Abp;
using Abp.Dependency;
using Abp.Notifications;
using FirebaseAdmin.Messaging;
using System.Threading.Tasks;

namespace TACHYON.Firebases
{
    public interface IFirebaseNotifier : ITransientDependency
    {
        FirebaseMessaging messaging { get; set; }
        Task PushNotification(string notificationName, NotificationData data, params long[] userIds);


    }
}