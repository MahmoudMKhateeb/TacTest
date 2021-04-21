using Abp;
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
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.TachyonPriceOffers;

namespace TACHYON.Notifications
{
    public class AppNotifier : TACHYONDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly UserManager _userManager;

        public AppNotifier(INotificationPublisher notificationPublisher, INotificationSubscriptionManager notificationSubscriptionManager, UserManager userManager)
        {
            _notificationPublisher = notificationPublisher;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _userManager = userManager;
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
            UserIdentifier user = new UserIdentifier(invoice.TenantId, _userManager.GetAdminByTenantIdAsync(invoice.TenantId).Id);

            await _notificationPublisher.PublishAsync(AppNotificationNames.InvoiceShipperGenerated, notificationData,
                userIds: new[] { user });


        }




        public async Task NewSubmitInvoiceGenerated(GroupPeriod groupPeriod)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewGroupPeriodsGenerated",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["groupid"] = groupPeriod.Id;
            UserIdentifier user = new UserIdentifier(groupPeriod.TenantId, _userManager.GetAdminByTenantIdAsync(groupPeriod.TenantId).Id);

            await _notificationPublisher.PublishAsync(AppNotificationNames.GroupPeriodsGenerated, notificationData,
                userIds: new[] { user });
        }

        public async Task SubmitInvoiceOnClaim(UserIdentifier User, GroupPeriod groupPeriod)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceOnClaim",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["groupid"] = groupPeriod.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.SubmitInvoiceOnClaim, notificationData, userIds: new[] { User });
        }

        public async Task SubmitInvoiceOnAccepted(UserIdentifier User, GroupPeriod groupPeriod)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceOnAccepted",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["groupid"] = groupPeriod.Id;

            await _notificationPublisher.PublishAsync(AppNotificationNames.SubmitInvoiceOnAccepted, notificationData, userIds: new[] { User });
        }
        public async Task SubmitInvoiceOnRejected(UserIdentifier User, GroupPeriod groupPeriod)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "SubmitInvoiceOnRejected",
                    TACHYONConsts.LocalizationSourceName
                )
            );
            notificationData["groupid"] = groupPeriod.Id;
            notificationData["reason"] = groupPeriod.RejectedReason;
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


        public async Task NotifyDriverWhenAssignToTrip(ShippingRequestTrip Trip)
        {
            if (Trip.AssignedDriverUserId.HasValue) return;
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

                notificationData["ShipimentNo"] = Trip.ShippingRequestId;
                notificationData["driver"] = driver;
                notificationData["source"] = Trip.OriginFacilityFk.Address;
                notificationData["destination"] = Trip.DestinationFacilityFk.Address;

             await _notificationPublisher.PublishAsync(AppNotificationNames.DriverRejectTrip, notificationData, userIds:new[] { await GetAdminUser (Trip.ShippingRequestFk.CarrierTenantId) });
        }

        public async Task DriverAcceptTrip(ShippingRequestTrip Trip, string driver)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    L("DriverAcceptTrip"),
                    TACHYONConsts.LocalizationSourceName
                )
            );

            notificationData["ShipimentNo"] = Trip.ShippingRequestId;
            notificationData["driver"] = driver;
            notificationData["source"] = Trip.OriginFacilityFk.Address;
            notificationData["destination"] = Trip.DestinationFacilityFk.Address;
            await _notificationPublisher.PublishAsync(AppNotificationNames.DriverAcceptTrip, notificationData, userIds: new[] { await GetAdminUser(Trip.ShippingRequestFk.CarrierTenantId) });
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
            notificationData["ShipimentNo"] = Request.Id;

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
            notificationData["ShipimentNo"] = Pricing.RequestId;
            notificationData["clientname"] = Pricing.Carrirer.companyName;
            var user = new UserIdentifier(Pricing.TenantId, Pricing.CreatorUserId.Value);
            await _notificationPublisher.PublishAsync(AppNotificationNames.DriectRequestCarrierRespone, notificationData, userIds: new[] { user });
        }
       
        public async Task TachyonDealerOfferCreated(TachyonPriceOffer offer,ShippingRequest request)
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
            notificationData["requestid"] = offer.Id;
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
            notificationData["requestid"] = offer.Id;
            notificationData["clientname"] = offer.ShippingRequestFk.Tenant.companyName;
           var user=new UserIdentifier(offer.TenantId, offer.CreatorUserId.Value);

            await _notificationPublisher.PublishAsync(AppNotificationNames.TachyonDealOfferAcceptedByShipper, notificationData, userIds: new[] { user });
        }

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
            notificationData["requestid"] = shippingRequest.Id;
            notificationData["clientname"] = shippingRequest.Tenant.companyName;

            await _notificationPublisher.PublishAsync(AppNotificationNames.ShippingRequestNotifyCarrirerWhenShipperAccepted, notificationData, userIds: new[] { await GetAdminUser(shippingRequest.CarrierTenantId) });
        }
        #endregion
        #endregion

        #region Helper
        private async Task<UserIdentifier> GetAdminUser(int? TenantId)
        {
            var user = await _userManager.GetAdminByTenantIdAsync(TenantId.Value);
            return new UserIdentifier(TenantId, user.Id);
        }
        #endregion
    }
}