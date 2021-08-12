using Abp.Domain.Repositories;
using FirebaseAdmin.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Mobile;
using System.Linq;
using Abp.Timing;
using Abp.Localization;
using System;
using Abp.Configuration;
using Abp;
using System.Globalization;

namespace TACHYON.Firebases
{
    public  class FirebaseNotifier: TACHYONServiceBase, IFirebaseNotifier
    {

        public FirebaseMessaging messaging { get; set; }
        private readonly IRepository<UserDeviceToken> _userDeviceToken;
        private readonly ISettingManager _settingManager;
        public FirebaseNotifier(IRepository<UserDeviceToken> userDeviceToken, ISettingManager settingManager)
        {
            messaging = FirebaseMessaging.DefaultInstance;
            _userDeviceToken = userDeviceToken;
            _settingManager = settingManager;

        }
        public async Task General
            (
                UserIdentifier user,
                Dictionary<string, string> data,
                string clickAction,
                string localizeKey
            )
        {
            string Title = L(localizeKey, GetCulture(user));
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = Title,
                },
                Data = data
            };
            await SendMessage(user.UserId, message, clickAction);
        }
        /// <summary>
        /// Send notification to driver when the carrier assign new trip
        /// </summary>
        /// <param name="user"></param>
        /// <param name="TripId"></param>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public async Task PushNotificationToDriverWhenAssignTrip(UserIdentifier user, string TripId,string wayBillNumber)
        {
            string Title = L("NewTripAssign", GetCulture(user), wayBillNumber);
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
            await SendMessage(user.UserId, message, "ViewComingLoadInfoActivity");
        }
        /// <summary>
        /// Reminder the driver before one day 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task ReminderDriverForTrip(UserIdentifier user, string TripId)
        {
            string Title = L("DriverTripReminder", GetCulture(user));
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
            await SendMessage(user.UserId, message, "ViewComingLoadInfoActivity");
        }

        /// <summary>
        /// Reminder the driver before one day 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="TripId"></param>
        /// <returns></returns>
        public async Task TripChanged(UserIdentifier user, string TripId)
        {
            string Title = L("TripDataChanged");
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = Title,
                },
                Data = new Dictionary<string, string>()
                {
                    ["id"] = TripId,
                    ["changed"] = "true"
                }
            };
            await SendMessage(user.UserId, message, "ViewtripchangedActivity");
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
                catch 
                {
                    // todo add log here 
                }
            }
        }

        private Task<IQueryable<UserDeviceToken>> GetUserDevices(long UserId)
        {
            return Task.FromResult(_userDeviceToken.GetAll().Where(x => x.UserId == UserId && (!x.ExpireDate.HasValue || x.ExpireDate >= Clock.Now)));
        }
        private  CultureInfo GetCulture(UserIdentifier user)
        {
           return new CultureInfo(_settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, user.TenantId, user.UserId, true)) ;
        }
        #endregion
    }
}
