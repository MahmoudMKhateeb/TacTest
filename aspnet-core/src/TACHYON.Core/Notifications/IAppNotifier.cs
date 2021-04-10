using Abp;
using Abp.Localization;
using Abp.Notifications;
using System;
using System.Collections.Generic;
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
    public interface IAppNotifier
    {

        #region Tachyon notifications

        Task AssignDriverToTruck(UserIdentifier argsUser, long truckId);
        Task UpdateShippingRequestPrice(UserIdentifier argsUser, long shippingRequestId, decimal price);
        Task AcceptShippingRequestPrice(long shippingRequestId, bool isAccepted);
        Task RejectShippingRequest(UserIdentifier argsUser, long shippingRequestId);
        Task NewInvoiceShipperGenerated(Invoice invoice);    
        Task NewSubmitInvoiceGenerated(GroupPeriod groupPeriod);
        Task SubmitInvoiceOnClaim(UserIdentifier User,GroupPeriod groupPeriod);
        Task SubmitInvoiceOnAccepted(UserIdentifier User, GroupPeriod groupPeriod);
        Task SubmitInvoiceOnRejected(UserIdentifier User, GroupPeriod groupPeriod);
        Task ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage);


        Task SomeTrucksCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName);
        Task CreateBidRequest(UserIdentifier argsUser, long shippingRequestBidId);
         Task TenantDocumentFileUpdate(DocumentFile documentFile);

        Task CancelBidRequest(UserIdentifier argsUser, long shippingRequestId,long shippingRequestBidId);

        Task AcceptShippingRequestBid(UserIdentifier argsUser, long shippingRequestBidId);
        Task ShippingRequestAsBidWithSameTruckAsync(UserIdentifier[] argsUser, long shippingRequestId);
        Task StartShippment(UserIdentifier argsUser, long TripId, string PickupFacilityName);
        Task ShipperShippingRequestFinish(UserIdentifier argsUser, ShippingRequest Request);
        #region Trips
        Task ShipperShippingRequestTripNotifyDriverWhenAssignTrip(UserIdentifier argsUser, ShippingRequestTrip Trip);
        Task NotifyDriverWhenAssignToTrip(ShippingRequestTrip Trip);
        Task DriverRejectTrip(ShippingRequestTrip Trip, string driver);
        Task DriverAcceptTrip(ShippingRequestTrip Trip, string driver);
        #endregion

        #region Accident
        Task ShippingRequestAccidentsOccure(List<UserIdentifier> Users, Dictionary<string, object> data);
        Task ShippingRequestTripCancelByAccident(List<UserIdentifier> Users, ShippingRequestTrip trip,User UserCancel);
        #endregion
        #endregion
        #region TachyonDeal
        Task SendDriectRequestForCarrier(int? TenantId,ShippingRequest Request);
        Task DriectRequestCarrierRespone(ShippingRequestsCarrierDirectPricing Pricing);

        Task TachyonDealOfferRejectedByShipper(TachyonPriceOffer offer);
        Task TachyonDealOfferAccepByShipper(TachyonPriceOffer offer);

        #endregion
        #region Shipping Request
        Task ShippingRequestNotifyCarrirerWhenShipperAccepted(ShippingRequest shippingRequest);
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
        Task AcceptedSubmittedDocument(UserIdentifier argsUser, DocumentFile documentFile);
        Task RejectedSubmittedDocument(UserIdentifier argsUser, DocumentFile documentFile);
    }
}