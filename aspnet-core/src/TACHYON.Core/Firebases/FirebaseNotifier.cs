using Abp;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Timing;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Mobile;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Firebases
{
    public class FirebaseNotifier : TACHYONDomainServiceBase, IFirebaseNotifier
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
        public async Task PushNotificationToDriverWhenAssignTrip(UserIdentifier user, string TripId, string wayBillNumber)
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
            string Title = L("TripDataChanged", TripId);
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

        public async Task TripUpdated(NotifyTripUpdatedInput input)
        {
            string msgTitle = L("CarrierTripUpdatedNotificationMessage",
                input.WaybillNumber, input.UpdatedBy);

            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = msgTitle,
                },
                Data = new Dictionary<string, string>()
                {
                    ["id"] = input.TripId.ToString(),
                    ["changed"] = "true"
                }
            };
            await SendMessage(input.DriverIdentifier.UserId, message, "ViewtripchangedActivity");
        }

        #region Helper
        private async Task SendMessage(long UserId, Message message, string ClickAction)
        {

            var devices = await GetUserDevices(UserId);

            foreach (var device in devices)
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

        private async Task<List<UserDeviceToken>> GetUserDevices(long UserId)
        {
            var devices = await _userDeviceToken.GetAll()
                .Where(x => x.UserId == UserId
                            && (!x.ExpireDate.HasValue
                                || x.ExpireDate >= Clock.Now)).ToListAsync();
            return devices;
        }

        private CultureInfo GetCulture(UserIdentifier user)
        {
            return new CultureInfo(_settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, user.TenantId, user.UserId, true));
        }
        #endregion
    }
}