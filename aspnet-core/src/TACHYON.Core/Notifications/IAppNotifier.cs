using Abp;
using Abp.Localization;
using Abp.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Invoices;
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
    public interface IAppNotifier
    {

        #region Tachyon notifications

        Task AssignDriverToTruck(UserIdentifier argsUser, long truckId);
        Task UpdateShippingRequestPrice(UserIdentifier argsUser, long shippingRequestId, decimal price);
        Task AcceptShippingRequestPrice(long shippingRequestId, bool isAccepted);
        Task RejectShippingRequest(UserIdentifier argsUser, long shippingRequestId);
        Task NewInvoiceShipperGenerated(Invoice invoice);
        Task NewSubmitInvoiceGenerated(SubmitInvoice submitInvoice);
        Task SubmitInvoiceOnClaim(UserIdentifier User, SubmitInvoice submitInvoice);
        Task SubmitInvoiceOnAccepted(UserIdentifier User, SubmitInvoice submitInvoice);
        Task SubmitInvoiceOnRejected(UserIdentifier User, SubmitInvoice submitInvoice);
        Task ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(int? TenantId, int Percentage);
        Task NewBalanceAddedToShippper(int ShipperTenantId, decimal Amount);

        Task SomeTrucksCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName);
        Task CreateBidRequest(UserIdentifier argsUser, long shippingRequestBidId);
        Task UpdateBidRequest(UserIdentifier argsUser, long shippingRequestBidId);
        Task TenantDocumentFileUpdate(DocumentFile documentFile);

        Task CancelBidRequest(UserIdentifier argsUser, long shippingRequestId, long shippingRequestBidId);

        Task AcceptShippingRequestBid(UserIdentifier argsUser, long shippingRequestBidId);
        Task ShippingRequestAsBidWithSameTruckAsync(UserIdentifier[] argsUser, long shippingRequestId);
        Task StartShippment(UserIdentifier argsUser, long TripId, string PickupFacilityName);
        Task ShipperShippingRequestFinish(UserIdentifier argsUser, ShippingRequest Request);

        #region Trips


        Task NotifyDriverWhenAssignTrip(int tripId, params UserIdentifier[] drivers);

        Task NotifyDriverWhenUnassignedTrip(int tripId, string waybillNumber, params UserIdentifier[] drivers);

        Task DriverRejectTrip(ShippingRequestTrip Trip, string driver);
        Task DriverAcceptTrip(ShippingRequestTrip Trip, string driver);
        Task CarrierTripNeedAccept(ShippingRequestTrip Trip);
        Task TMSTripNeedAccept(ShippingRequestTrip Trip);
        Task NotifyCarrierWhenTripHasAttachment(int tripId, int? carrierTenantId, bool hasAttachment);
        Task NotifyCarrierWhenTripNeedsDeliverNote(int tripId, int? carrierTenantId);
        Task NotificationWhenTripDetailsChanged(ShippingRequestTrip trip, User currentuser);
        Task NotifyCarrierWithDriverGpsOff(UserIdentifier argsUser, User driver);
        #endregion

        #region Accident

        Task ShippingRequestAccidentsOccure(List<UserIdentifier> Users, Dictionary<string, object> data);
        Task ShippingRequestTripCancelByAccident(List<UserIdentifier> Users, ShippingRequestTrip trip, User UserCancel);
        Task ShippingRequestTripCanceled(List<UserIdentifier> Users, ShippingRequestTrip trip, string tenantName);
        Task ShippingRequestTripRejectCancelByTachyonDealer(List<UserIdentifier> Users, ShippingRequest request);
        Task ShippingRequestTripNeedsCancelApproval(List<UserIdentifier> Users, ShippingRequest request);
        #endregion

        #region ShippingRequest

        #region Offers
        Task ShippingRequestSendOfferWhenAddPrice(PriceOffer offer, string carrier);
        Task ShippingRequestSendOfferWhenUpdatePrice(PriceOffer offer, string carrier);


        Task ShipperAcceptedOffer(PriceOffer offer);
        Task TMSAcceptedOffer(PriceOffer offer);
        Task RejectedOffer(PriceOffer offer, string RejectedBy);
        Task PendingOffer(PriceOffer offer);

        Task SendDriectRequest(string FromTenant, int? ToTenant, long id);
        Task DeclineDriectRequest(string FromTenant, int? ToTenant, long id);

        Task CancelShipment(long id, string reason, string cancelBy, UserIdentifier toUser);

        #endregion

        #endregion

        #endregion

        #region TachyonDeal
        Task SendDriectRequestForCarrier(int? TenantId, ShippingRequest Request);
        Task DriectRequestCarrierRespone(ShippingRequestsCarrierDirectPricing Pricing);
        Task TachyonDealerOfferCreated(TachyonPriceOffer offer, ShippingRequest request);
        Task TachyonDealOfferRejectedByShipper(TachyonPriceOffer offer);
        Task TachyonDealOfferAccepByShipper(TachyonPriceOffer offer);

        #endregion

        #region Shipping Request

        Task ShippingRequestNotifyCarrirerWhenShipperAccepted(ShippingRequest shippingRequest);
        Task ShipperReminderToCompleteTrips(long shippingRequestId, UserIdentifier userIds);

        #endregion

        Task WelcomeToTheApplicationAsync(User user);

        Task NewUserRegisteredAsync(User user);

        Task NewTenantRegisteredAsync(string tenancyName);

        Task GdprDataPrepared(UserIdentifier user, Guid binaryObjectId);

        Task SendMessageAsync(UserIdentifier user, string message,
            NotificationSeverity severity = NotificationSeverity.Info);

        Task SendMessageAsync(UserIdentifier user, LocalizableString localizableMessage,
            IDictionary<string, object> localizableMessageData = null,
            NotificationSeverity severity = NotificationSeverity.Info);

        Task TenantsMovedToEdition(UserIdentifier user, string sourceEditionName, string targetEditionName);

        Task SomeUsersCouldntBeImported(UserIdentifier user, string fileToken, string fileType, string fileName);
        Task DocumentFileBeforExpiration(UserIdentifier argsUser, Guid documentFileId, int expirationAlertDays);
        Task DocumentFileExpiration(UserIdentifier argsUser, Guid documentFileId);
        Task AcceptedSubmittedDocument(UserIdentifier argsUser, DocumentFile documentFile);
        Task RejectedSubmittedDocument(UserIdentifier argsUser, DocumentFile documentFile);
        Task NotifyShipperWhenTripUpdated(NotifyTripUpdatedInput input);
        Task NotifyCarrierWhenTripUpdated(NotifyTripUpdatedInput input);

        /// <summary>
        /// Notify All Users When Trip Updated
        /// <list type="bullet|number|table">
        ///<item><term>1-Shipper</term></item>
        ///<item><term>2-Carrier</term></item>
        ///<item><term>3-TachyonDealer</term></item>
        /// </list>
        /// Note That Driver Not Notified By This Method
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task NotifyAllWhenTripUpdated(NotifyTripUpdatedInput input);

        Task NotifyTachyonDealWhenTripUpdated(NotifyTripUpdatedInput input);

        Task NotifyTripUpdated(ShippingRequestTrip trip);

        Task DriverReminderForTrip(UserIdentifier user, string tripId);

        Task NotifyDriverOnlyWhenTripUpdated(int tripId, string waybillNumber, params UserIdentifier[] drivers);
    }
}