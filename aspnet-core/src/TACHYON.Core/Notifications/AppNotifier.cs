using Abp;
using Abp.Localization;
using Abp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.MultiTenancy;

namespace TACHYON.Notifications
{
    public class AppNotifier : TACHYONDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;


        public AppNotifier(INotificationPublisher notificationPublisher, INotificationSubscriptionManager notificationSubscriptionManager)
        {
            _notificationPublisher = notificationPublisher;
            _notificationSubscriptionManager = notificationSubscriptionManager;
        }

        #region Tachyon Notifications

        public async Task AssignDriverToTruck(UserIdentifier argsUser, long truckId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("AssignDriverToTruckNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["truckId"] = truckId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.AssignDriverToTruck, notificationData, userIds: new[] { argsUser });
        }


        public async Task UpdateShippingRequestPrice(UserIdentifier argsUser, long shippingRequestId, decimal price)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("UpdateShippingRequestPriceNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["shippingRequestId"] = shippingRequestId;
            notificationData["price"] = price;
            await _notificationPublisher.PublishAsync(AppNotificationNames.UpdateShippingRequestPrice, notificationData, userIds: new[] { argsUser });
        }

        public async Task AcceptShippingRequestPrice(long shippingRequestId, bool isAccepted)
        {
            var subscriptions = await _notificationSubscriptionManager.GetSubscriptionsAsync(AppNotificationNames.AcceptShippingRequestPrice);
            var userIds = subscriptions.Select(subscription => new UserIdentifier(subscription.TenantId, subscription.UserId)).ToArray();

            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("AcceptShippingRequestPriceNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["shippingRequestId"] = shippingRequestId;
            notificationData["isAccepted"] = isAccepted;
            await _notificationPublisher.PublishAsync(AppNotificationNames.AcceptShippingRequestPrice, notificationData, userIds: userIds);
        }

        public async Task RejectShippingRequest(UserIdentifier argsUser, long shippingRequestId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("RejectShippingRequestNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["shippingRequestId"] = shippingRequestId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.RejectShippingRequest, notificationData, userIds: new[] { argsUser });
        }
        public Task SomeTrucksCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName)
        {
            return SendNotificationAsync(AppNotificationNames.DownloadInvalidImportUsers, user,
                new LocalizableString(
                    "ClickToSeeInvalidTrucks",
                    TACHYONConsts.LocalizationSourceName
                ),
                new Dictionary<string, object>
                {
                    { "fileToken", fileToken },
                    { "fileType", fileType },
                    { "fileName", fileName }
                });
        }
        public async Task CreateBidRequest(UserIdentifier argsUser, long shippingRequestBidId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("CreateBidRequestNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                    )
                );
            notificationData["shippingRequestBidId"] = shippingRequestBidId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.CreateShippingRequestBid,
                notificationData,
                userIds: new[] { argsUser });
        }
        public async Task CancelBidRequest(UserIdentifier argsUser, long shippingRequestId,long shippingRequestBidId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("CancelBidRequestNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                    )
                );
            notificationData["shippingRequestId"] = shippingRequestId;
            notificationData["shippingRequestBidId"] = shippingRequestBidId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.CancelShippingRequestBid,
                notificationData,
                userIds: new[] { argsUser });
        }
        public async Task AcceptShippingRequestBid(UserIdentifier argsUser, long shippingRequestId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("AcceptShippingRequestNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                    )
                );
            notificationData["shippingRequestId"] = shippingRequestId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.AcceptShippingRequestBid,
                notificationData, 
                severity:NotificationSeverity.Success,
                userIds: new[] { argsUser });

        }

        public async Task ShippingRequestAsBidWithSameTruckAsync(UserIdentifier[] argsUser, long shippingRequestId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShippingRequestAsBidWithSameTruckNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                    )
                );
            notificationData["shippingRequestId"] = shippingRequestId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestAsBidWithSameTruck,
                notificationData,
                userIds: argsUser );
        }
        /// <summary>
        /// For documentFiles befor file expiration date 
        /// </summary>
        /// <param name="argsUser"></param>
        /// <param name="documentFileId"></param>
        /// <param name="expirationAlertDays">number of days remaining befor expiration date</param>
        /// <returns></returns>
        public async Task DocumentFileBeforExpiration(UserIdentifier argsUser, Guid documentFileId, int expirationAlertDays)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DocumentFileBeforExpirationNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["documentFileId"] = documentFileId;
            notificationData["expirationAlertDays"] = expirationAlertDays;
            await _notificationPublisher.PublishAsync(AppNotificationNames.DocumentFileBeforExpiration, notificationData, userIds: new[] { argsUser });
        }


        /// <summary>
        /// When document file expiration date 
        /// </summary>
        /// <param name="argsUser"></param>
        /// <returns></returns>
        public async Task DocumentFileExpiration(UserIdentifier argsUser, Guid documentFileId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DocumentFileExpirationNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["documentFileId"] = documentFileId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.DocumentFileExpiration, notificationData, userIds: new[] { argsUser });
        }


        public async Task TenantDocumentFileUpdate( DocumentFile documentFile)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TenantDocumentFileUpdateNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["documentFileId"] = documentFile.Id;
            notificationData["documentFileTenantId"] = documentFile.TenantId;

            await _notificationPublisher.PublishAsync(AppNotificationNames.TenantDocumentFileUpdate, notificationData);
        }
        #endregion
        public async Task WelcomeToTheApplicationAsync(User user)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.WelcomeToTheApplication,
                new MessageNotificationData(L("WelcomeToTheApplicationNotificationMessage")),
                severity: NotificationSeverity.Success,
                userIds: new[] { user.ToUserIdentifier() }
            );
        }


        public async Task NewUserRegisteredAsync(User user)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewUserRegisteredNotificationMessage",
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["userName"] = user.UserName;
            notificationData["emailAddress"] = user.EmailAddress;

            await _notificationPublisher.PublishAsync(AppNotificationNames.NewUserRegistered, notificationData,
                tenantIds: new[] { user.TenantId });
        }

        public async Task NewTenantRegisteredAsync(Tenant tenant)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewTenantRegisteredNotificationMessage",
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["tenancyName"] = tenant.TenancyName;
            await _notificationPublisher.PublishAsync(AppNotificationNames.NewTenantRegistered, notificationData);
        }

        public async Task GdprDataPrepared(UserIdentifier user, Guid binaryObjectId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "GdprDataPreparedNotificationMessage",
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["binaryObjectId"] = binaryObjectId;

            await _notificationPublisher.PublishAsync(AppNotificationNames.GdprDataPrepared, notificationData,
                userIds: new[] { user });
        }

        //This is for test purposes
        public async Task SendMessageAsync(UserIdentifier user, string message,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.SimpleMessage,
                new MessageNotificationData(message),
                severity: severity,
                userIds: new[] { user }
            );
        }

        public Task SendMessageAsync(UserIdentifier user, LocalizableString localizableMessage,
            IDictionary<string, object> localizableMessageData = null,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            return SendNotificationAsync(AppNotificationNames.SimpleMessage, user, localizableMessage,
                localizableMessageData, severity);
        }

        protected async Task SendNotificationAsync(string notificationName, UserIdentifier user,
            LocalizableString localizableMessage, IDictionary<string, object> localizableMessageData = null,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            var notificationData = new LocalizableMessageNotificationData(localizableMessage);
            if (localizableMessageData != null)
            {
                foreach (var pair in localizableMessageData)
                {
                    notificationData[pair.Key] = pair.Value;
                }
            }

            await _notificationPublisher.PublishAsync(notificationName, notificationData, severity: severity,
                userIds: new[] { user });
        }

        public Task TenantsMovedToEdition(UserIdentifier user, string sourceEditionName, string targetEditionName)
        {
            return SendNotificationAsync(AppNotificationNames.TenantsMovedToEdition, user,
                new LocalizableString(
                    "TenantsMovedToEditionNotificationMessage",
                    TACHYONConsts.LocalizationSourceName
                ),
                new Dictionary<string, object>
                {
                    {"sourceEditionName", sourceEditionName},
                    {"targetEditionName", targetEditionName}
                });
        }

        public Task<TResult> TenantsMovedToEdition<TResult>(UserIdentifier argsUser, int sourceEditionId,
            int targetEditionId)
        {
            throw new NotImplementedException();
        }

        public Task SomeUsersCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName)
        {
            return SendNotificationAsync(AppNotificationNames.DownloadInvalidImportUsers, user,
                new LocalizableString(
                    "ClickToSeeInvalidUsers",
                    TACHYONConsts.LocalizationSourceName
                ),
                new Dictionary<string, object>
                {
                    { "fileToken", fileToken },
                    { "fileType", fileType },
                    { "fileName", fileName }
                });
        }


    }
}