using Abp.Domain.Repositories;
using FirebaseAdmin.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Mobile;
using System.Linq;
using Abp.Timing;

namespace TACHYON.Firebases
{
    public  class FirebaseNotifier: IFirebaseNotifier
    {

        public FirebaseMessaging messaging { get; set; }
        private readonly IRepository<UserDeviceToken> _userDeviceToken;

        public FirebaseNotifier(IRepository<UserDeviceToken> userDeviceToken)
        {
            messaging = FirebaseMessaging.DefaultInstance;
            _userDeviceToken = userDeviceToken;

        }

        public async Task PushNotificationToDriverWhenAssignTrip(long driverId,string TripId)
        {
            foreach (var device in _userDeviceToken.GetAll().Where(x => x.UserId == driverId && x.ExpireDate>= Clock.Now))
            {
                try
                {
                    var message = new Message()
                    {
                        Notification = new Notification
                        {
                            Title = "NewTripAssign",
                        },
                        Token = device.Token,
                        Data = new Dictionary<string, string>()
                        {
                            ["id"] = TripId
                        },
                        Android = new AndroidConfig()
                        {
                            Notification = new AndroidNotification
                            {
                                Title = "NewTripAssign",
                            },
                            Data = new Dictionary<string, string>()
                            {
                                ["id"] = TripId
                            }
                        }
                    };
                     await messaging.SendAsync(message);
                }
                catch
                {

                }

            }
        }
    }
}
