﻿using System.ComponentModel.Design.Serialization;

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
        public const string BrokerEditionName = "broker";
        public const string CarrierSaasEditionName = "carrierassaas";
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
            "https://play.google.com/store/apps/details?id=com.tachyonhub.tachyondriverapp";

        public const string IOSAppLink = "https://apps.apple.com/us/app/tachyon-driver/id1617802156";

        #endregion

        #region SMS

        public const string SMSOTP = "SMSOTP";
        public const string SMSShippingRequestReceiverCode = "ShippingRequestReceiverCode";
        public const string SMSSaasShippingRequestReceiverCode = "SMSSaasShippingRequestReceiverCode";
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

        #region Vases
        public const string AppointmentVasName = "Appointment";
        public const string ClearanceVasName = "Custom Clearance";
        #endregion
        #endregion

        public const string OthersDisplayName = "Others";



        public const string ProposalTemplateFullNamespace =
            "TACHYON.Net.Emailing.PricePackageTemplates.TachyonProposal.docx";        
        public const string AppendixTemplateFullNamespace =
            "TACHYON.Net.Emailing.PricePackageTemplates.TachyonAppendix.docx";

        public const string ProposalTemplateCompanyName = "$$CompanyName$$";
        public const string ProposalTemplateDate = "$$Date$$";
        public const string ProposalTemplateTruckType = "$$TruckType$$";
        public const string ProposalTemplateRouteType = "$$RouteType$$";
        public const string ProposalTemplateShippingType = "$$ShippingType$$";
        public const string ProposalTemplateScopeOverview = "$$ScopeOverview$$";
        public const string ProposalTemplateNotes = "$$Notes$$";
        
        
        public const string AppendixTemplateContractNumber = "$$ContractNumber$$";
        public const string AppendixTemplateContractDate = "$$ContractDate$$";
        public const string AppendixTemplateVersion = "$$Version$$";
        public const string AppendixTemplateAppendixDate = "$$AppendixDate$$";
        public const string AppendixTemplateTruckTypes = "$$TruckTypes$$";
        public const string AppendixTemplateTransportTypes = "$$TransportTypes$$";
        public const string AppendixTemplateRouteTypes = "$$RouteTypes$$";
        public const string AppendixTemplateClientName = "$$ClientName$$";
        public const string AppendixTemplateScopeOverview = "$$ScopeOverview$$";
        public const string AppendixTemplateNotes = "$$Notes$$";
    }
}