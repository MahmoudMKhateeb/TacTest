namespace TACHYON
{
    public class TACHYONConsts
    {
        public const string LocalizationSourceName = "TACHYON";
        public const string DefaultLanguage = "en";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;

        public const bool AllowTenantsToChangeEmailSettings = false;

        public const string Currency = "USD";

        public const string CurrencySign = "$";

        public const string AbpApiClientUserAgent = "AbpApiClient";

        //BidStatus consts
        //public const int ShippingRequestStatusStandBy = 1;

        //public const int ShippingRequestStatusOnGoing = 2;

        //public const int ShippingRequestStatusClosed = 3;

        //public const int ShippingRequestStatusCanceled = 4;

        //Route Type consts
        public const int SingleDropRoutType = 1;

        public const int TwoWaysRoutType = 2;

        public const int MultipleDropsRoutType = 3;

        //Pickup consts
        public const int PickupPickingType = 1;
        public const int DropoffPickingType = 2;

        // Note:
        // Minimum accepted payment amount. If a payment amount is less then that minimum value payment progress will continue without charging payment
        // Even though we can use multiple payment methods, users always can go and use the highest accepted payment amount.
        //For example, you use Stripe and PayPal. Let say that stripe accepts min 5$ and PayPal accepts min 3$. If your payment amount is 4$.
        // User will prefer to use a payment method with the highest accept value which is a Stripe in this case.
        public const decimal MinimumUpgradePaymentAmount = 1M;

        #region Tachyon consts

        public const int MaxDocumentFileBytesUserFriendlyValue = 5;
        public const string ShipperEdtionName = "shipper";
        public const string CarrierEdtionName = "carrier";
        public const string TachyonDealerEdtionName = "Tachyon";

        //BidStatus consts
        public const int ShippingRequestStatusStandBy = 1;

        public const int ShippingRequestStatusOnGoing = 2;

        public const int ShippingRequestStatusClosed = 3;

        public const int ShippingRequestStatusCanceled = 4;

        //Trucks consts
        /// <summary>
        /// active
        /// </summary>
        public const int TruckDefualtStatusId = 1;

        /// <summary>
        /// publicTransport
        /// </summary>
        public const int TruckDefualtPlateTypeId = 1;

        /// <summary>
        ///  used for mapping fields for Truck Istimara document in import from excel
        /// </summary>
        public const string TruckIstimaraDocumentTypeSpecialConstant = "TruckIstimara";

        /// <summary>
        ///  used for mapping fields for Truck Insurance document in import from excel
        /// </summary>
        public const string TruckInsuranceDocumentTypeSpecialConstant = "TruckInsurance";

        public const string DriverIqamaDocumentTypeSpecialConstant = "DriverIqama";

        public const string TripAttachmentDocumentTypeSpecialConstant = "TripAttachment";


        #region Mobile

        public const string AndroidAppLink =
            "https://play.google.com/store/apps/details?id=com.trustangle.tachyondriver";

        public const string IOSAppLink = "https://apps.apple.com/us/app/tachyon-driver/id1561722003";

        #endregion

        #region SMS

        public const string SMSOTP = "SMSOTP";
        public const string SMSShippingRequestReceiverCode = "ShippingRequestReceiverCode";
        public const string DriverWelcomeMessage = "DriverWelcomeMessageRegistrationNotification";

        #endregion

        #region Signalr triggers

        #region Tracking

        public const string TriggerTrackingAccepted = "app.tracking.accepted";
        public const string TriggerTrackingStarted = "app.tracking.started";
        public const string TriggerTrackingChanged = "app.tracking.changed";
        public const string TriggerTrackingShipmentDelivered = "app.tracking.Shipment.Delivered";

        #endregion

        #endregion

        #endregion

        public const string OthersDisplayName = "Others";

        #region WorkFlow Const

        public const int PickUpRoutPointWorkflowVersion = 0;
        public const int DropOfRoutPointWorkflowVersion = 1;
        public const int DropOfWithDeliveryNoteRoutPointWorkflowVersion = 2;

        #endregion
    }
}