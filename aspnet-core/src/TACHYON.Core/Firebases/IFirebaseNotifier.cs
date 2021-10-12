using Abp;
using Abp.Dependency;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Firebases
{
    public interface IFirebaseNotifier : ITransientDependency
    {
        FirebaseMessaging messaging { get; set; }
        Task General(UserIdentifier user, Dictionary<string, string> data, string clickAction, string localizeKey);
        Task PushNotificationToDriverWhenAssignTrip(UserIdentifier user, string TripId, string wayBillNumber);
        Task ReminderDriverForTrip(UserIdentifier user, string TripId);
        Task TripChanged(UserIdentifier user, string TripId);
        Task TripUpdated(NotifyTripUpdatedInput input);


    }
}