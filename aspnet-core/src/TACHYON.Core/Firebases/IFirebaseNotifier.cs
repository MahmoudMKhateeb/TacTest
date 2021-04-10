using Abp.Dependency;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Firebases
{
   public interface IFirebaseNotifier : ITransientDependency
    {
        FirebaseMessaging messaging { get; set; }
        Task PushNotificationToDriverWhenAssignTrip(long driverId, string TripId);

    }
}
