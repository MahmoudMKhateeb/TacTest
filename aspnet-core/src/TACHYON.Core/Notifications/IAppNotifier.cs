using Abp;
using Abp.Localization;
using Abp.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.MultiTenancy;

namespace TACHYON.Notifications
{
    public interface IAppNotifier
    {

        #region Tachyon notifications

        Task AssignDriverToTruck(UserIdentifier argsUser, Guid truckId);
        Task UpdateShippingRequestPrice(UserIdentifier argsUser, long shippingRequestId, decimal price);
        Task AcceptShippingRequestPrice(long shippingRequestId, bool isAccepted);
        Task RejectShippingRequest(UserIdentifier argsUser, long shippingRequestId);

        Task SomeTrucksCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName);
         Task TenantDocumentFileUpdate(DocumentFile documentFile);


        Task AcceptShippingRequestBid(UserIdentifier argsUser, long shippingRequestBidId);
        Task CreateShippingRequestAsBid(UserIdentifier[] argsUser, long shippingRequestId);
        #endregion
        Task WelcomeToTheApplicationAsync(User user);

        Task NewUserRegisteredAsync(User user);

        Task NewTenantRegisteredAsync(Tenant tenant);

        Task GdprDataPrepared(UserIdentifier user, Guid binaryObjectId);

        Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info);

        Task SendMessageAsync(UserIdentifier user, LocalizableString localizableMessage, IDictionary<string, object> localizableMessageData = null, NotificationSeverity severity = NotificationSeverity.Info);

        Task TenantsMovedToEdition(UserIdentifier user, string sourceEditionName, string targetEditionName);

        Task SomeUsersCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName);
        Task DocumentFileBeforExpiration(UserIdentifier argsUser, Guid documentFileId, int expirationAlertDays);
        Task DocumentFileExpiration(UserIdentifier argsUser, Guid documentFileId);
    }
}