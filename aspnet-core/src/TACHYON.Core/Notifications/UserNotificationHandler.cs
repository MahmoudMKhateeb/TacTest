using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Handlers;
using Abp.Localization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;

namespace TACHYON.Notifications
{
    public class UserNotificationHandler : IAsyncEventHandler<NotificationPushedEventData>, ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IRepository<User,long> _userRepository;

        public UserNotificationHandler(
            ISettingManager settingManager,
            IUserEmailer userEmailer,
            IUnitOfWorkManager uowManager, 
             IRepository<User, long> userRepository)
        {
            _settingManager = settingManager;
            _userEmailer = userEmailer;
            _uowManager = uowManager;
            _userRepository = userRepository;
        }

        public async Task HandleEventAsync(NotificationPushedEventData eventData)
        {
            using var uow = _uowManager.Begin();

            // in the most cases, the length of `UserIdentifiers` is 1 
            foreach (UserIdentifier identifier in eventData.UserIdentifiers)
                await CheckAndSendNotificationEmail(identifier, eventData);
            
            await uow.CompleteAsync();
        }

        private async Task CheckAndSendNotificationEmail(UserIdentifier userIdentifier,NotificationPushedEventData eventData)
        {
            var subscribedNotificationEmails = await _settingManager
                .GetSettingValueForUserAsync(AppSettings.UserManagement.SubscribedNotificationEmails,
                    userIdentifier);
            if (string.IsNullOrEmpty(subscribedNotificationEmails)) return;

            if (subscribedNotificationEmails.Contains(eventData.Name))
            {
                using (_uowManager.Current.SetTenantId(userIdentifier.TenantId, true))
                {
                    var emailAddress = await _userRepository.GetAll().Where(x => x.Id == userIdentifier.UserId)
                        .Select(x => x.EmailAddress).FirstOrDefaultAsync();

                    if (string.IsNullOrEmpty(emailAddress)) return;

                    await _userEmailer.SendNotificationByEmail(emailAddress, eventData.DisplayName, eventData.Message);
                }
            }
        }
    }
}