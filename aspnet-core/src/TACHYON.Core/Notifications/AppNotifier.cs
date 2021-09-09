using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Invoices;
using TACHYON.Invoices.Groups;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.TachyonPriceOffers;

namespace TACHYON.Notifications
{
    public class AppNotifier : TACHYONDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly UserManager _userManager;
        private readonly IRepository<User, long> _userRepo;
        private readonly IRepository<Tenant> _tenantsRepository;

        public AppNotifier(INotificationPublisher notificationPublisher, INotificationSubscriptionManager notificationSubscriptionManager, UserManager userManager, IRepository<User, long> userRepo, IRepository<Tenant> tenantsRepository)
        {
            _notificationPublisher = notificationPublisher;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _userManager = userManager;
            _userRepo = userRepo;
            _tenantsRepository = tenantsRepository;
        }

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

        public async Task UpdateBidRequest(UserIdentifier argsUser, long shippingRequestBidId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("UpdateBidRequestNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                    )
                );
            notificationData["shippingRequestBidId"] = shippingRequestBidId;
            await _notificationPublisher.PublishAsync(AppNotificationNames.UpdateShippingRequestBid,
                notificationData,
                userIds: new[] { argsUser });
        }
        public async Task CancelBidRequest(UserIdentifier argsUser, long shippingRequestId, long shippingRequestBidId)
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
                severity: NotificationSeverity.Success,
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
                userIds: argsUser);
        }

        public async Task StartShippment(UserIdentifier argsUser, long TripId, string PickupFacilityName)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("StartShippmentNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["tripId"] = TripId;
            notificationData["PickupFacilityName"] = PickupFacilityName;
            await _notificationPublisher.PublishAsync(AppNotificationNames.StartShippment,
                notificationData,
                userIds: new[] { argsUser });
        }


        public async Task ShipperShippingRequestFinish(UserIdentifier argsUser, ShippingRequest Request)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShipperShippingRequestFinishNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["requestid"] = Request.Id;
            await _notificationPublisher.PublishAsync(AppNotificationNames.ShipperShippingRequestFinish,
                notificationData,
                userIds: new[] { argsUser });
        }

        //document Files

        //document Files
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


        public async Task TenantDocumentFileUpdate(DocumentFile documentFile)
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


        public async Task AcceptedSubmittedDocument(UserIdentifier argsUser, DocumentFile documentFile)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DocumentFileAcceptedNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["documentFileId"] = documentFile.Id;
            notificationData["documentFileName"] = documentFile.Name;
            await _notificationPublisher.PublishAsync(AppNotificationNames.AcceptedSubmittedDocument, notificationData, userIds: new[] { argsUser });
        }

        public async Task RejectedSubmittedDocument(UserIdentifier argsUser, DocumentFile documentFile)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DocumentFileRejectedNotificationMessage"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["documentFileId"] = documentFile.Id;
            notificationData["documentFileName"] = documentFile.Name;
            await _notificationPublisher.PublishAsync(AppNotificationNames.RejectedSubmittedDocument, notificationData, userIds: new[] { argsUser });
        }

        public async Task NotifyShipperWhenTripUpdated(NotifyTripUpdatedInput input)
        {
            var tenantAdmin = await GetTenantAdminUser(input.ShipperTenantId);

            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                L("ShipperTripUpdatedNotificationMessage",
                    input.WaybillNumber, input.UpdatedBy),
                TACHYONConsts.LocalizationSourceName))
            {
                Properties = new Dictionary<string, object>() { { "updatedTripId", input.TripId } }
            };

            await _notificationPublisher.PublishAsync(AppNotificationNames.NotifyShipperWhenTripUpdated, notificationData, userIds: new[] { tenantAdmin });
        }

        public async Task NotifyCarrierWhenTripUpdated(NotifyTripUpdatedInput input)
        {
            var tenantAdmin = await GetTenantAdminUser(input.CarrierTenantId);

            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("CarrierTripUpdatedNotificationMessage",
            input.WaybillNumber, input.UpdatedBy),
                    TACHYONConsts.LocalizationSourceName))
            {
                Properties = new Dictionary<string, object>() { { "updatedTripId", input.TripId } }
            };

            await _notificationPublisher.PublishAsync(AppNotificationNames.NotifyCarrierWhenTripUpdated, notificationData, userIds: new[] { tenantAdmin });
        }

        public async Task NotifyTachyonDealWhenTripUpdated(NotifyTripUpdatedInput input)
        {
            var tenantAdmin = await GetAdminTachyonDealerAsync();

            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TachyonDealerTripUpdatedNotificationMessage",
                        input.WaybillNumber, input.UpdatedBy),
                    TACHYONConsts.LocalizationSourceName))
            {
                Properties = new Dictionary<string, object>() { { "updatedTripId", input.TripId } }
            };

            await _notificationPublisher.PublishAsync(AppNotificationNames.NotifyTachyonDealWhenTripUpdated, notificationData, userIds: new[] { tenantAdmin });
        }

        public async Task NotifyAllWhenTripUpdated(NotifyTripUpdatedInput input)
        {
            await NotifyShipperWhenTripUpdated(input);
            await NotifyCarrierWhenTripUpdated(input);
            await NotifyTachyonDealWhenTripUpdated(input);
        }

        #region Invoices
        public async Task NewInvoiceShipperGenerated(Invoice invoice)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewInvoiceShipperGenerated",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["invoiceid"] = invoice.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.InvoiceShipperGenerated, notificationData,
                userIds: new[] { await GetAdminUser(invoice.TenantId) });

        }




        public async Task NewSubmitInvoiceGenerated(SubmitInvoice submitInvoice)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceGenerated",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = submitInvoice.Id;
            UserIdentifier user = new UserIdentifier(submitInvoice.TenantId, _userManager.GetAdminByTenantIdAsync(submitInvoice.TenantId).Id);

            await _notificationPublisher.PublishAsync(AppNotificationNames.SubmitInvoiceGenerated, notificationData,
                userIds: new[] { user });
        }

        public async Task SubmitInvoiceOnClaim(UserIdentifier User, SubmitInvoice submitInvoice)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceOnClaim",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = submitInvoice.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.SubmitInvoiceOnClaim, notificationData, userIds: new[] { User });
        }

        public async Task SubmitInvoiceOnAccepted(UserIdentifier User, SubmitInvoice submitInvoice)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceOnAccepted",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = submitInvoice.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.SubmitInvoiceOnAccepted, notificationData, userIds: new[] { User });
        }
        public async Task SubmitInvoiceOnRejected(UserIdentifier User, SubmitInvoice submitInvoice)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceOnRejected",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = submitInvoice.Id;
            notificationData["reason"] = submitInvoice.RejectedReason;
            await _notificationPublisher.PublishAsync(AppNotificationNames.SubmitInvoiceOnRejected, notificationData, userIds: new[] { User });
        }
        public async Task ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["Percentage"] = Percentage;
            UserIdentifier user = new UserIdentifier(TenantId, _userManager.GetAdminByTenantIdAsync(TenantId.Value).Id);

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage, notificationData, userIds: new[] { user });
        }
        #endregion

        #region Trips
        public async Task ShipperShippingRequestTripNotifyDriverWhenAssignTrip(UserIdentifier argsUser, ShippingRequestTrip Trip)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L(AppNotificationNames.ShipperShippingRequestTripNotifyDriverWhenAssignTrip),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["id"] = Trip.Id;
            await _notificationPublisher.PublishAsync(AppNotificationNames.ShipperShippingRequestTripNotifyDriverWhenAssignTrip, notificationData, userIds: new[] { argsUser });
        }

        public async Task ShipperShippingRequestTripNotifyDriverWhenUnassignedTrip(UserIdentifier argsUser, ShippingRequestTrip Trip)
        { // Need To Add Localization Message String
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L(AppNotificationNames.ShipperShippingRequestTripNotifyDriverWhenUnassignedTrip),
                    TACHYONConsts.LocalizationSourceName
                )
            )
            {
                ["id"] = Trip.Id
            };

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShipperShippingRequestTripNotifyDriverWhenUnassignedTrip, notificationData, userIds: new[] { argsUser });
        }

        public async Task NotifyDriverWhenAssignToTrip(ShippingRequestTrip Trip)
        {
            if (!Trip.AssignedDriverUserId.HasValue) return;
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("NotifyDriverWhenAssignToTrip"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            var user = await _userManager.FindByIdAsync(Trip.AssignedDriverUserId.Value.ToString());

            notificationData["id"] = Trip.Id;
            await _notificationPublisher.PublishAsync(AppNotificationNames.NotifyDriverWhenAssignToTrip, notificationData, userIds: new[] { new UserIdentifier(user.TenantId, user.Id) });
        }


        public async Task DriverRejectTrip(ShippingRequestTrip Trip, string driver)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DriverRejectTrip"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["id"] = Trip.ShippingRequestId;
            notificationData["driver"] = driver;
            notificationData["source"] = Trip.OriginFacilityFk.Address;
            notificationData["destination"] = Trip.DestinationFacilityFk.Address;

            await _notificationPublisher.PublishAsync(AppNotificationNames.DriverRejectTrip, notificationData, userIds: new[] { await GetAdminUser(Trip.ShippingRequestFk.CarrierTenantId) });
        }

        public async Task DriverAcceptTrip(ShippingRequestTrip Trip, string driver)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DriverAcceptTrip"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["id"] = Trip.ShippingRequestId;
            notificationData["driver"] = driver;
            notificationData["source"] = Trip.OriginFacilityFk.Address;
            notificationData["destination"] = Trip.DestinationFacilityFk.Address;
            await _notificationPublisher.PublishAsync(AppNotificationNames.DriverAcceptTrip, notificationData, userIds: new[] { await GetAdminUser(Trip.ShippingRequestFk.CarrierTenantId) });
        }
        public async Task CarrierTripNeedAccept(ShippingRequestTrip Trip)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("CarrierTripNeedAccept"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["waybillnumber"] = Trip.WaybillNumber;
            notificationData["driver"] = Trip.AssignedDriverUserFk.FullName;
            //notificationData["source"] = Trip.OriginFacilityFk.Address;
            // notificationData["destination"] = Trip.DestinationFacilityFk.Address;
            await _notificationPublisher.PublishAsync(AppNotificationNames.CarrierTripNeedAccept, notificationData, userIds: new[] { await GetAdminUser(Trip.AssignedDriverUserFk.TenantId) });
        }
        public async Task TMSTripNeedAccept(ShippingRequestTrip Trip)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TMSTripNeedAccept"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["waybillnumber"] = Trip.WaybillNumber;
            notificationData["driver"] = Trip.AssignedDriverUserFk.FullName;
            notificationData["carrier"] = Trip.ShippingRequestFk.CarrierTenantFk.Name;
            // notificationData["source"] = Trip.OriginFacilityFk.Address;
            //notificationData["destination"] = Trip.DestinationFacilityFk.Address;
            var tmsUser = await _userManager.GetAdminTachyonDealerAsync();
            await _notificationPublisher.PublishAsync(AppNotificationNames.TMSTripNeedAccept, notificationData, userIds: new[] { new UserIdentifier(tmsUser.TenantId, tmsUser.Id) });
        }
        public async Task NotificationWhenTripDetailsChanged(ShippingRequestTrip trip, User currentuser)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("NotificationWhenTripDetailsChanged"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["waybillnumber"] = trip.WaybillNumber;

            List<UserIdentifier> users = new List<UserIdentifier>();


            if (trip.ShippingRequestFk.CreatorUserId != currentuser.Id)
            {
                users.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId, trip.ShippingRequestFk.CreatorUserId.Value));
            }
            if (trip.ShippingRequestFk.CarrierTenantId.HasValue && trip.ShippingRequestFk.CarrierTenantId != currentuser.TenantId)
            {
                var carrier = await _userManager.GetAdminByTenantIdAsync(trip.ShippingRequestFk.CarrierTenantId.Value);
                users.Add(new UserIdentifier(carrier.TenantId, carrier.Id));
            }
            else if (trip.ShippingRequestFk.IsTachyonDeal)
            {
                var tms = await _userManager.GetAdminTachyonDealerAsync();
                if (tms.Id != currentuser.Id)
                    users.Add(new UserIdentifier(tms.TenantId.Value, tms.Id));
            }

            await _notificationPublisher.PublishAsync(AppNotificationNames.NotificationWhenTripDetailsChanged, notificationData, userIds: users.ToArray());
        }

        public async Task NotifyCarrierWhenTripHasAttachment(int tripId, int? carrierTenantId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TripHasAttachment"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["TripId"] = tripId;
            if (carrierTenantId != null)
            {
                var user = await _userManager.GetAdminByTenantIdAsync(carrierTenantId.Value);
                await _notificationPublisher.PublishAsync(AppNotificationNames.TripHasAttachment, notificationData, userIds: new[] { new UserIdentifier(carrierTenantId, user.Id) });
            }
        }

        public async Task NotifyCarrierWhenTripNeedsDeliverNote(int tripId, int? carrierTenantId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TripNeedsDeliveryNote"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["TripId"] = tripId;
            if (carrierTenantId != null)
            {
                var user = await _userManager.GetAdminByTenantIdAsync(carrierTenantId.Value);
                await _notificationPublisher.PublishAsync(AppNotificationNames.TripNeedsDeliveryNote, notificationData, userIds: new[] { new UserIdentifier(carrierTenantId, user.Id) });
            }
        }

        #endregion
        #region Accident
        public async Task ShippingRequestAccidentsOccure(List<UserIdentifier> Users, Dictionary<string, object> data)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShippingRequestAccidentsOccure"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            foreach (var d in data)
            {
                notificationData[d.Key] = d.Value;
            }

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestAccidents, notificationData, userIds: Users.ToArray());
        }

        public async Task ShippingRequestTripCancelByAccident(List<UserIdentifier> Users, ShippingRequestTrip trip, User UserCancel)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShippingRequestCancelTripByAccidents"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = trip.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestCancelByTripAccidents, notificationData, userIds: Users.ToArray());
        }
        #endregion

        #region TachyonDeal
        public async Task SendDriectRequestForCarrier(int? TenantId, ShippingRequest Request)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("SendDriectRequestForCarrier"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = Request.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.SendDriectRequestForCarrier, notificationData, tenantIds: new[] { TenantId });
        }
        public async Task DriectRequestCarrierRespone(ShippingRequestsCarrierDirectPricing Pricing)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DriectRequestCarrierRespone"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = Pricing.RequestId;
            notificationData["clientname"] = Pricing.Carrier.companyName;
            var user = new UserIdentifier(Pricing.TenantId, Pricing.CreatorUserId.Value);
            await _notificationPublisher.PublishAsync(AppNotificationNames.DriectRequestCarrierRespone, notificationData, userIds: new[] { user });
        }

        public async Task TachyonDealerOfferCreated(TachyonPriceOffer offer, ShippingRequest request)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TachyonDealOfferCreated"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["offerid"] = offer.Id;
            var user = new UserIdentifier(request.TenantId, request.CreatorUserId.Value);

            await _notificationPublisher.PublishAsync(AppNotificationNames.TachyonDealOfferCreated, notificationData, userIds: new[] { user });

        }
        public async Task TachyonDealOfferRejectedByShipper(TachyonPriceOffer offer)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TachyonDealOfferRejectedByShipper"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = offer.Id;
            notificationData["clientname"] = offer.ShippingRequestFk.Tenant.companyName;

            var user = new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value);

            await _notificationPublisher.PublishAsync(AppNotificationNames.TachyonDealOfferRejectedByShipper, notificationData, userIds: new[] { user });
        }

        public async Task TachyonDealOfferAccepByShipper(TachyonPriceOffer offer)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("TachyonDealOfferAcceptedByShipper"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = offer.Id;
            notificationData["clientname"] = offer.ShippingRequestFk.Tenant.companyName;
            var user = new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value);

            await _notificationPublisher.PublishAsync(AppNotificationNames.TachyonDealOfferAcceptedByShipper, notificationData, userIds: new[] { user });
        }

        #endregion

        #region ShippingRequest
        #region Offers
        public async Task ShippingRequestSendOfferWhenAddPrice(PriceOffer offer, string carrier)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShippingRequestSendOfferWhenAddPrice"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["offerid"] = offer.Id;
            notificationData["id"] = offer.ShippingRequestId;
            notificationData["carrier"] = carrier;
            List<UserIdentifier> users = new List<UserIdentifier>();
            if (!offer.ShippingRequestFK.IsTachyonDeal)
            {
                users.Add(new UserIdentifier(offer.ShippingRequestFK.TenantId, offer.ShippingRequestFK.CreatorUserId.Value));
            }
            else
            {
                var user = await _userManager.GetAdminTachyonDealerAsync();
                users.Add(new UserIdentifier(user.TenantId.Value, user.Id));
            }
            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestSendOfferWhenAddPrice, notificationData, userIds: users.ToArray());
        }


        public async Task ShippingRequestSendOfferWhenUpdatePrice(PriceOffer offer, string carrier)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShippingRequestSendOfferWhenUpdatePrice"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["offerid"] = offer.Id;
            notificationData["id"] = offer.ShippingRequestId;
            notificationData["carrier"] = carrier;
            List<UserIdentifier> users = new List<UserIdentifier>();
            if (!offer.ShippingRequestFK.IsTachyonDeal)
            {
                users.Add(new UserIdentifier(offer.ShippingRequestFK.TenantId, offer.ShippingRequestFK.CreatorUserId.Value));
            }
            else
            {
                var user = await _userManager.GetAdminTachyonDealerAsync();
                users.Add(new UserIdentifier(user.TenantId.Value, user.Id));
            }
            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestSendOfferWhenUpdatePrice, notificationData, userIds: users.ToArray());
        }

        public async Task ShipperAcceptedOffer(PriceOffer offer)
        {
            var notificationData = new LocalizableMessageNotificationData(
                                    new LocalizableString(L("ShipperAcceptedOffers"),
                                    TACHYONConsts.LocalizationSourceName));
            notificationData["offerid"] = offer.Id;
            notificationData["id"] = offer.ShippingRequestId;
            notificationData["shipper"] = offer.ShippingRequestFK.Tenant.Name;
            List<UserIdentifier> users = new List<UserIdentifier>();
            users.Add(new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value));

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShipperAcceptedOffer, notificationData, userIds: users.ToArray());

        }
        public async Task TMSAcceptedOffer(PriceOffer offer)
        {
            var notificationData = new LocalizableMessageNotificationData(
                                    new LocalizableString(L("TMSAcceptedOffer"),
                                    TACHYONConsts.LocalizationSourceName));
            notificationData["offerid"] = offer.Id;
            notificationData["id"] = offer.ShippingRequestId;
            notificationData["name"] = L("TachyonManageService");
            List<UserIdentifier> users = new List<UserIdentifier>();
            users.Add(new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value));


            await _notificationPublisher.PublishAsync(AppNotificationNames.TMSAcceptedOffer, notificationData, userIds: users.ToArray());

        }

        public async Task SendDriectRequest(string FromTenant, int? ToTenant, long id)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("SendDirectRequest"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = id;
            notificationData["client"] = FromTenant;
            await _notificationPublisher.PublishAsync(AppNotificationNames.SendDriectRequest, notificationData, userIds: new[] { await GetAdminUser(ToTenant) });
        }
        public async Task DeclineDriectRequest(string FromTenant, int? ToTenant, long id)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DeclineDriectRequest"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = id;
            notificationData["client"] = FromTenant;
            await _notificationPublisher.PublishAsync(AppNotificationNames.DeclineDriectRequest, notificationData, userIds: new[] { await GetAdminUser(ToTenant) });
        }
        public async Task CancelShipment(long id, string reason, string cancelBy, UserIdentifier toUser)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("CancelShipment"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = id;
            notificationData["cancelby"] = cancelBy;
            await _notificationPublisher.PublishAsync(AppNotificationNames.CancelShipment, notificationData, userIds: new[] { toUser });
        }

        public async Task RejectedOffer(PriceOffer offer, string RejectedBy)
        {
            var notificationData = new LocalizableMessageNotificationData(
                                    new LocalizableString(L("RejectedOffer"),
                                    TACHYONConsts.LocalizationSourceName));
            notificationData["id"] = offer.Id;
            //notificationData["id"] = offer.ShippingRequestId;
            notificationData["reason"] = offer.RejectedReason;
            notificationData["rejectedby"] = RejectedBy;
            List<UserIdentifier> users = new List<UserIdentifier>();
            users.Add(new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value));

            await _notificationPublisher.PublishAsync(AppNotificationNames.RejectedOffer, notificationData, userIds: users.ToArray());
        }
        public async Task PendingOffer(PriceOffer offer)
        {
            var notificationData = new LocalizableMessageNotificationData(
                                    new LocalizableString(L("PendingOffer"),
                                    TACHYONConsts.LocalizationSourceName));
            notificationData["id"] = offer.Id;
            List<UserIdentifier> users = new List<UserIdentifier>();
            users.Add(new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value));
            //L("TachyonManageService")
            await _notificationPublisher.PublishAsync(AppNotificationNames.PendingOffer, notificationData, userIds: users.ToArray());
        }

        #endregion
        #endregion

        #region Shipping Request
        public async Task ShippingRequestNotifyCarrirerWhenShipperAccepted(ShippingRequest shippingRequest)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShippingRequestNotifyCarrirerWhenShipperAccepted"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = shippingRequest.Id;
            notificationData["clientname"] = shippingRequest.Tenant.companyName;

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestNotifyCarrirerWhenShipperAccepted, notificationData, userIds: new[] { await GetAdminUser(shippingRequest.CarrierTenantId) });
        }
        public async Task ShipperReminderToCompelteTrips(UserIdentifier user, ShippingRequest shippingRequest)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("ShipperReminderToCompelteTrips"),
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["id"] = shippingRequest.Id;
            await _notificationPublisher.PublishAsync(AppNotificationNames.ShipperReminderToCompelteTrips, notificationData, userIds: new[] { user });
        }
        #endregion
        #endregion

        #region Helper
        private async Task<UserIdentifier> GetAdminUser(int? TenantId)
        {
            var user = await _userManager.GetAdminByTenantIdAsync(TenantId.Value);
            var identifier = new UserIdentifier(TenantId, user.Id);

            return identifier;
        }

        [UnitOfWork]
        protected virtual async Task<UserIdentifier> GetTenantAdminUser(int tenantId)
        {

            DisableTenancyFilters();

            var user = await _userRepo.FirstOrDefaultAsync(x =>
                        x.TenantId == tenantId && x.UserName.Equals(AbpUserBase.AdminUserName));

            return new UserIdentifier(tenantId, user.Id);
        }

        [UnitOfWork]
        protected virtual async Task<UserIdentifier> GetAdminTachyonDealerAsync()
        {
            var tenant = await _tenantsRepository.FirstOrDefaultAsync(x => x.Edition.Name.ToLower() == AppConsts.TachyonEditionName.ToLower());
            return await GetTenantAdminUser(tenant.Id);
        }

        #endregion
    }
}