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

        public const string ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage = "App.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage";

        public const string StartShippment = "App.StartShippment";

        public const string ShipperShippingRequestFinish = "App.Shipper.ShippingRequest.Finish";

        #region Trip
        public const string ShipperShippingRequestTripNotifyDriverWhenAssignTrip = "App.ShipperShippingRequestTripNotifyDriverWhenAssignTrip";
        public const string NotifyDriverWhenAssignToTrip = "App.NotifyDriverWhenAssignToTrip";
        public const string DriverRejectTrip = "App.DriverRejectTrip";
        public const string DriverAcceptTrip = "App.DriverAcceptTrip";
        #endregion
        #region Accident
        public const string ShippingRequestAccidents = "App.ShippingRequest.Accident";
        public const string ShippingRequestCancelByTripAccidents = "App.ShippingRequest.trip.Accident.Cancel";

        #endregion
        #region TachyonDeal
        public const string SendDriectRequestForCarrier = "App.SendDriectRequestForCarrier";
        public const string DriectRequestCarrierRespone = "App.DriectRequestCarrierRespone";
        public const string TachyonDealOfferRejectedByShipper = "App.TachyonDealOfferRejectedByShipper";
        public const string TachyonDealOfferAcceptedByShipper = "App.TachyonDealOfferAcceptedByShipper";
        public const string TachyonDealOfferCreated = "App.TachyonDealOfferCreated";
        #endregion
        #region Shipping Request
        public const string ShippingRequestNotifyCarrirerWhenShipperAccepted = "App.ShippingRequestNotifyCarrirerWhenShipperAccepted";
        public const string ShipperReminderToCompelteTrips = "App.ShipperReminderToCompelteTrips";
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
        #endregion

    }
}