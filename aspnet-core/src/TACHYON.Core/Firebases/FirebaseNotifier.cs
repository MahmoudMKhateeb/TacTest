using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Timing;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Mobile;

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
    
       
        
        public async Task PushNotification(
            string notificationName,
            NotificationData data = null,
            params long[] userIds)
        {
            
            var tokens = await GetUsersDevices(userIds);
            List<Message> msgList = new List<Message>();
            foreach (string token in tokens)
            {

                Message message = new Message {Token = token};
                message.Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification()
                    {
                        Title = message.Notification.Title,
                        Body = notificationName,

                    },
                    Data = data?.Properties.ToDictionary(x=> x.Key,x=> x.Value.ToString())
                };
                msgList.Add(message);
            }

            try
            {
               await messaging.SendAllAsync(msgList);
            }
            catch (FirebaseMessagingException ex)
            {
                Logger.Error("Error When Send Notification",ex);
            }
        }
        
        #region Helper

        private async Task<string[]> GetUsersDevices(params long[] userIds)
        {
            var devices = await _userDeviceToken.GetAll()
                .Where(x => userIds.Contains(x.UserId)
                            && (!x.ExpireDate.HasValue
                                || x.ExpireDate >= Clock.Now))
                .Select(x=> x.Token).ToArrayAsync();
            return devices;
        }
        #endregion
    }
}