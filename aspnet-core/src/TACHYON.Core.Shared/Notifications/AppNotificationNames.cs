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
        public const string CancelShippingRequestBid = "app.CancelShippingRequestBid";
        public const string DocumentFileBeforExpiration = "App.DocumentFileBeforExpiration";
        public const string DocumentFileExpiration = "App.DocumentFileExpiration";
        public const string TenantDocumentFileUpdate = "App.TenantDocumentFileUpdate";
        public const string AcceptedSubmittedDocument = "App.AcceptedSubmittedDocument";
        public const string RejectedSubmittedDocument = "App.RejectedSubmittedDocument";
        public const string InvoiceShipperGenerated = "App.InvoiceShipperGenerated";


        public const string StartShippment = "App.StartShippment";

        public const string ShipperShippingRequestFinish = "App.Shipper.ShippingRequest.Finish";
        #endregion

    }
}