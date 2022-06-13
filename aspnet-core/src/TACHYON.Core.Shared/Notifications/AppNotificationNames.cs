using System.Security.Cryptography.X509Certificates;

namespace TACHYON.Notifications
{
    /// <summary>
    /// Constants for notification names used in this application.
    /// </summary>
    public static class AppNotificationNames
    {
        public const string SimpleMessage = "App.SimpleMessage";
        public const string WelcomeToTheApplication = "App.WelcomeToTheApplication";
        public const string NewUserRegistered = "App.NewUserRegistered";
        public const string NewTenantRegistered = "App.NewTenantRegistered";
        public const string GdprDataPrepared = "App.GdprDataPrepared";
        public const string TenantsMovedToEdition = "App.TenantsMovedToEdition";
        public const string DownloadInvalidImportUsers = "App.DownloadInvalidImportUsers";


        #region Tychon notifications

        public const string AssignDriverToTruck = "App.AssignDriverToTruck";


        public const string UpdateShippingRequestPrice = "App.UpdateShippingRequestPrice";
        public const string AcceptShippingRequestPrice = "App.AcceptShippingRequestPrice";
        public const string RejectShippingRequest = "App.RejectShippingRequest";
        public const string AcceptShippingRequestBid = "App.AcceptShippingRequestBid";
        public const string ShippingRequestAsBidWithSameTruck = "App.ShippingRequestAsBidWithSameTruck";
        public const string ShippingRequestAsBidWithMatchingPricePackage = "App.ShippingRequestAsBidWithMatchingPricePackage";
        public const string CreateShippingRequestBid = "app.CreateShippingRequestBid";
        public const string UpdateShippingRequestBid = "app.UpdateShippingRequestBid";
        public const string CancelShippingRequestBid = "app.CancelShippingRequestBid";
        public const string DocumentFileBeforExpiration = "App.DocumentFileBeforExpiration";
        public const string DocumentFileExpiration = "App.DocumentFileExpiration";
        public const string TenantDocumentFileUpdate = "App.TenantDocumentFileUpdate";
        public const string AcceptedSubmittedDocument = "App.AcceptedSubmittedDocument";
        public const string RejectedSubmittedDocument = "App.RejectedSubmittedDocument";
        public const string InvoiceShipperGenerated = "App.InvoiceShipperGenerated";
        public const string SubmitInvoiceGenerated = "App.SubmitInvoiceGenerated";

        public const string SubmitInvoiceOnClaim = "App.SubmitInvoiceOnClaim";
        public const string SubmitInvoiceOnAccepted = "App.SubmitInvoiceOnAccepted";
        public const string SubmitInvoiceOnRejected = "App.SubmitInvoiceOnRejected";
        public const string NewBalanceAddedToShippper = "App.NewBalanceAddedToShippper";
        public const string NewCreaditOrDebitNoteGenerated = "App.CreaditOrDebitNoteGenerated";

        public const string ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage =
            "App.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage";

        public const string StartShippment = "App.StartShippment";

        public const string ShipperShippingRequestFinish = "App.Shipper.ShippingRequest.Finish";
        public const string DriverGpsOff = "App.DriverGpsOff";
        public const string PricePackageOfferWasCreated = "App.PricePackageOfferWasCreated";
        public const string CarrierAcceptPricePackageOffer = "App.CarrierAcceptPricePackageOffer";

        #region Trip

        public const string NotifyDriverWhenAssignTrip = "App.ShipperShippingRequestTripNotifyDriverWhenAssignTrip";

        public const string NotifyDriverWhenUnassignedTrip =
            "App.ShipperShippingRequestTripNotifyDriverWhenUnassignedTrip";

        public const string NotifyDriverWhenAssignToTrip = "App.NotifyDriverWhenAssignToTrip";
        public const string DriverRejectTrip = "App.DriverRejectTrip";
        public const string DriverAcceptTrip = "App.DriverAcceptTrip";
        public const string CarrierTripNeedAccept = "App.CarrierTripNeedAccept";
        public const string TMSTripNeedAccept = "App.TMSTripNeedAccept";
        public const string TripHasAttachment = "App.TripHasAttachment";
        public const string TripNeedsDeliveryNote = "App.NeedsDeliveryNote";
        public const string NotificationWhenTripDetailsChanged = "App.NotificationWhenTripDetailsChanged";
        public const string NotifyShipperWhenTripUpdated = "App.NotifyShipperWhenTripUpdated";
        public const string NotifyCarrierWhenTripUpdated = "App.NotifyCarrierWhenTripUpdated";
        public const string NotifyTachyonDealWhenTripUpdated = "App.NotifyTachyonDealWhenTripUpdated";
        public const string ShippingRequestTripCanceled = "App.ShippingRequest.ShippingRequestTripCanceled";
        public const string ShippingRequestTripRejectCancelByTachyonDealer = "App.ShippingRequest.ShippingRequestTripRejectCancelByTachyonDealer";
        public const string ShippingRequestTripNeedsCancelApproval = "App.ShippingRequest.ShippingRequestTripNeedsCancelApproval";
        
        public const string NotifyOfferOwnerWhenSrUpdated = "App.NotifyOfferOwnerWhenShippingRequestUpdated";

        #endregion

        #region Accident

        public const string ShippingRequestAccidents = "App.ShippingRequest.Accident";
        public const string ShippingRequestCancelByTripAccidents = "App.ShippingRequest.trip.Accident.Cancel";
        public const string TripAccidentResolved = "App.Trip.Accident.Resolved";

        #endregion

        #region TachyonDeal

        public const string SendDriectRequestForCarrier = "App.SendDriectRequestForCarrier";
        public const string DriectRequestCarrierRespone = "App.DriectRequestCarrierRespone";
        public const string TachyonDealOfferRejectedByShipper = "App.TachyonDealOfferRejectedByShipper";
        public const string TachyonDealOfferAcceptedByShipper = "App.TachyonDealOfferAcceptedByShipper";
        public const string TachyonDealOfferCreated = "App.TachyonDealOfferCreated";

        #endregion

        #region Shipping Request

        public const string ShippingRequestNotifyCarrirerWhenShipperAccepted =
            "App.ShippingRequestNotifyCarrirerWhenShipperAccepted";

        public const string ShipperReminderToCompleteTrips = "App.ShipperReminderToCompleteTrips";

        #region ShippingRequestPostPriceUpdate

        public const string NotifyCarrierWhenPostPriceSrUpdated = "App.NotifyCarrierWhenPostPriceSrUpdated";
        public const string NotifyShipperForPostPriceSrUpdateAction = "App.NotifyShipperForPostPriceSrUpdateAction";
        public const string NotifyShipperWhenRequestChangePrice = "App.NotifyShipperWhenRequestChangePrice";

        #endregion

        #region Offer

        public const string ShippingRequestSendOfferWhenAddPrice = "App.ShippingRequestSendOfferWhenAddPrice";
        public const string ShippingRequestSendOfferWhenUpdatePrice = "App.ShippingRequestSendOfferWhenUpdatePrice";
        public const string ShipperAcceptedOffer = "App.ShipperAcceptedOffer";
        public const string TMSAcceptedOffer = "App.TMSAcceptedOffer";

        public const string SendDriectRequest = "App.SendDriectRequest";
        public const string DeclineDriectRequest = "App.DeclineDriectRequest";

        public const string CancelShipment = "App.CancelShipment";

        public const string RejectedOffer = "App.RejectedOffer";
        public const string PendingOffer = "App.PendingOffer";

        #endregion

        #endregion
        #region PriceOffer
        public const string SendPriceOfferToShipper = "App.SendPriceOfferToShipper";
        #endregion
        #region Mobile

        public const string DriverTripReminder = "App.DriverTripReminder";

        #endregion

        public const string NotifyShipperBeforApplyDetention = "App.NotifyShipperBeforApplyDetention";
        public const string NotifyShipperWhenApplyDetention = "App.NotifyShipperWhenApplyDetention";

        #endregion
    }
}