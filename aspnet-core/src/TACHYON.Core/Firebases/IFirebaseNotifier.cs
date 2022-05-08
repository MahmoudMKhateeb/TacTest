using Abp;
using Abp.Dependency;
using Abp.Notifications;
using FirebaseAdmin.Messaging;
using System.Threading.Tasks;

namespace TACHYON.Firebases
{
    public interface IFirebaseNotifier : ITransientDependency
    {
        FirebaseMessaging Messaging { get; set; }
        Task PushNotification(string notificationName,string msgTitle, NotificationData data = null, params long[] userIds);

    }
}