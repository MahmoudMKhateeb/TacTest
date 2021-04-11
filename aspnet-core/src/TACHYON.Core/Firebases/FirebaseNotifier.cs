using Abp.Domain.Repositories;
using FirebaseAdmin.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Mobile;
using System.Linq;
using Abp.Timing;
using Abp.Localization;
using System;

namespace TACHYON.Firebases
{
    public  class FirebaseNotifier: IFirebaseNotifier
    {

        public FirebaseMessaging messaging { get; set; }
        private readonly IRepository<UserDeviceToken> _userDeviceToken;
        private readonly ILocalizationManager _localizationManager;
        public FirebaseNotifier(IRepository<UserDeviceToken> userDeviceToken, ILocalizationManager localizationManager)
        {
            messaging = FirebaseMessaging.DefaultInstance;
            _userDeviceToken = userDeviceToken;
            _localizationManager = localizationManager;

        }

        public async Task PushNotificationToDriverWhenAssignTrip(long driverId, string TripId)
        {
            string Title = L("NewTripAssign");
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = Title,
                },
                Data = new Dictionary<string, string>()
                {
                    ["id"] = TripId,
                    ["tripId"] = TripId
                }
            };
            await SendMessage(driverId, message, "ViewComingLoadInfoActivity");
        }


        #region Helper
        private async Task SendMessage(long UserId, Message message,string ClickAction)
        {
            foreach (var device in await GetUserDevices(UserId))
            {
                try
                {
                    message.Token = device.Token;
                    message.Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Title = message.Notification.Title,
                            Body = message.Notification.Body,
                            ClickAction = ClickAction

                        },
                        Data = message.Data
                    };
                    await messaging.SendAsync(message);
                }
                catch (Exception e)
                {

                }
            }
        }
        private Task<IQueryable<UserDeviceToken>> GetUserDevices(long UserId)
        {
            return Task.FromResult(_userDeviceToken.GetAll().Where(x => x.UserId == UserId && (!x.ExpireDate.HasValue || x.ExpireDate >= Clock.Now)));
        }
        private string L(string name)
        {
            return _localizationManager.GetString(TACHYONConsts.LocalizationSourceName, "NewTripAssign");
        }
        #endregion
    }
}
