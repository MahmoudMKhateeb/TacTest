namespace TACHYON.Features
{
    public static class AppFeatures
    {
        public const string MaxUserCount = "App.MaxUserCount";
        public const string ChatFeature = "App.ChatFeature";
        public const string TenantToTenantChatFeature = "App.ChatFeature.TenantToTenant";
        public const string TenantToHostChatFeature = "App.ChatFeature.TenantToHost";
        public const string TestCheckFeature = "App.TestCheckFeature";
        public const string TestCheckFeature2 = "App.TestCheckFeature2";


        #region ######## Tachyon features #########

        public const string Shipper = "App.Shipper";
        public const string Carrier = "App.Carrier";
        public const string Pay = "App.Pay";
        public const string Receipt = "App.Receipt";
        public const string CMS = "App.CMS";
        public const string ShipperClients = "App.ShipperClients";
        public const string CarrierClients = "App.CarrierClients";
        public const string SaasCommission = "App.SaasCommission";
        public const string CanClientLoginToSystem = "App.CanClientLoginToSystem";
        public const string CreateDirectTrip = "App.CreateDirectTrip";
        public const string DocumentsManagement = "App.DocumentsManagement";
        public const string Receiver = "App.Receiver";
        public const string TachyonDealer = "App.TachyonDealer";
        public const string ShippingRequest = "App.shippingRequest";
        public const string MarketPlace = "App.MarketPlace";
        public const string OffersMarketPlace = "App.OffersMarketPlace";
        public const string SendDirectRequest = "App.SendDirectRequest";
        public const string SendTachyonDealShippingRequest = "App.SendTachyonDealShippingRequest";
        public const string CarrierAsASaas = "App.CarrierAsASaas";
        public const string CarrierAsSaasCommissionValue = "App.CarrierAsASaas.carrierAsSaasCommissionValue";
        public const string ShipperCreditLimit = "App.Shipper.CreditLimit";
        public const string ShipperPeriods = "App.Shipper.Periods";
        public const string CarrierPeriods = "App.Carrier.Periods";


        public const string TachyonDealerCommissionPercentage = "TachyonDealer.TachyonDealerCommissionPercentage";
        public const string TachyonDealerCommissionValue = "TachyonDealer.TachyonDealerCommissionValue";
        public const string TachyonDealerMinValueCommission = "TachyonDealer.TachyonDealerMinValueCommission";
        public const string TachyonDealerCommissionType = "App.Shipper.TachyonDealerCommissionType";


        public const string InvoicePaymentMethod = "App.Shipper.Invoice.Payment.Method";
        public const string InvoicePaymentMethodCrarrier = "App.Shipper.Invoice.Payment.MethodCrarrier";

        #region Comission

        #region Shipper between carrier

        #region Trip Comission
        //bidding
        public const string TripCommissionPercentage = "App.Shipper.TripCommissionPercentage";
        public const string TripCommissionValue = "App.Shipper.TripCommissionValue";
        public const string TripMinValueCommission = "App.Shipper.TripMinValueCommission";
        public const string TripCommissionType = "App.Shipper.TripCommissionType";

        public static string DirectRequestCommissionPercentage = "App.Shipper.TripDirectRequestCommissionPercentage";
        public static string DirectRequestCommissionValue = "App.Shipper.TripDirectRequestCommissionValue";
        public static string DirectRequestCommissionType = "App.Shipper.TripDirectRequestCommissionType";
        public static string DirectRequestCommissionMinValue = "App.Shipper.TripDirectRequestCommissionMinValue";

        #endregion

        #region Truck Commission
        public const string TruckCommissionPercentage = "App.Shipper.TruckCommissionPercentage";
        public const string TruckCommissionValue = "App.Shipper.TruckCommissionValue";
        public const string TruckMinValueCommission = "App.Shipper.TruckMinValueCommission";
        public const string TruckCommissionType = "App.Shipper.TruckCommissionType";

        public static string TruckDirectRequestCommissionPercentage = "App.Shipper.TruckDirectRequestCommissionPercentage";
        public static string TruckDirectRequestCommissionValue = "App.Shipper.TruckDirectRequestCommissionValue";
        public static string TruckDirectRequestCommissionType = "App.Shipper.TruckDirectRequestCommissionType";
        public static string TruckDirectRequestCommissionMinValue = "App.Shipper.TruckDirectRequestCommissionMinValue";

        #endregion

        #region Vas Comission

        public const string VasCommissionPercentage = "App.Shipper.VasCommissionPercentage";
        public const string VasCommissionValue = "App.Shipper.VasCommissionValue";
        public const string VasMinValueCommission = "App.Shipper.VasMinValueCommission";
        public const string VasCommissionType = "App.Shipper.VasCommissionType";


        public static string DirectRequestVasCommissionType = "App.Shipper.DirectRequestVASCommissionType";
        public static string DirectRequestVasCommissionPercentage = "App.Shipper.DirectRequestVASCommissionPercentage";
        public static string DirectRequestVasCommissionValue = "App.Shipper.DirectRequestVASCommissionValue";
        public static string DirectRequestVasCommissionMinValue = "App.Shipper.DirectRequestVASCommissionMinValue";

        #endregion

        #region Truck Vas Comission

        public const string TruckVasCommissionPercentage = "App.Shipper.TruckVasCommissionPercentage";
        public const string TruckVasCommissionValue = "App.Shipper.TruckVasCommissionValue";
        public const string TruckVasMinValueCommission = "App.Shipper.TruckVasMinValueCommission";
        public const string TruckVasCommissionType = "App.Shipper.TruckVasCommissionType";


        public static string TruckDirectRequestVasCommissionType = "App.Shipper.TruckDirectRequestVASCommissionType";
        public static string TruckDirectRequestVasCommissionPercentage = "App.Shipper.TruckDirectRequestVASCommissionPercentage";
        public static string TruckDirectRequestVasCommissionValue = "App.Shipper.TruckDirectRequestVASCommissionValue";
        public static string TruckDirectRequestVasCommissionMinValue = "App.Shipper.TruckDirectRequestVASCommissionMinValue";

        #endregion

        #endregion

        #region TachyonDealer

        #region Trip Comission

        public const string TachyonDealerTripCommissionPercentage = "App.TachyonDealer.TripCommissionPercentage";
        public const string TachyonDealerTripCommissionValue = "App.TachyonDealer.TripCommissionValue";
        public const string TachyonDealerTripMinValueCommission = "App.TachyonDealer.TripMinValueCommission";
        public const string TachyonDealerTripCommissionType = "App.TachyonDealer.TripCommissionType";

        #endregion

        #region Truck commission

        public const string TachyonDealerTruckCommissionPercentage = "App.TachyonDealer.TruckCommissionPercentage";
        public const string TachyonDealerTruckCommissionValue = "App.TachyonDealer.TruckCommissionValue";
        public const string TachyonDealerTruckMinValueCommission = "App.TachyonDealer.TruckMinValueCommission";
        public const string TachyonDealerTruckCommissionType = "App.TachyonDealer.TruckCommissionType";

        #endregion

        #region Vas Comission

        public const string TachyonDealerVasCommissionPercentage = "App.TachyonDealer.VasCommissionPercentage";
        public const string TachyonDealerVasCommissionValue = "App.TachyonDealer.VasCommissionValue";
        public const string TachyonDealerVasMinValueCommission = "App.TachyonDealer.VasMinValueCommission";
        public const string TachyonDealerVasCommissionType = "App.TachyonDealer.VasCommissionType";

        #endregion

        #region Truck Vas Comission

        public const string TachyonDealerTruckVasCommissionPercentage = "App.TachyonDealer.TruckVasCommissionPercentage";
        public const string TachyonDealerTruckVasCommissionValue = "App.TachyonDealer.TruckVasCommissionValue";
        public const string TachyonDealerTruckVasMinValueCommission = "App.TachyonDealer.TruckVasMinValueCommission";
        public const string TachyonDealerTruckVasCommissionType = "App.TachyonDealer.TruckVasCommissionType";

        #endregion

        #endregion

        #endregion

        public const string BayanIntegration = "App.BayanIntegration";
        public const string IntegrationWsl = "App.Integration.Wsl";
        public const string IntegrationWslVehicleRegistration = "App.Integration.Wsl.VehicleRegistration";
        public const string IntegrationWslDriverRegistration = "App.Integration.Wsl.DriverRegistration";
        public const string IntegrationWslTripRegistration = "App.Integration.Wsl.TripRegistration";

        public const string SaasRelatedCarrier = "App.Shipper.SaaSCarrier";
        public const string Saas = "App.Shipper.Saas";

        public const string NormalPricePackages = "App.NormalPricePackage";
        #endregion

        public const string Bidding = "App.Pay.Bidding";
        public const string CreateDirectRequest = "App.Pay.CreateDirectRequest";
        public const string CreateTmsRequest = "App.Pay.CreateTmsRequest";
        public const string PayPeriod = "App.Pay.PayPeriod";

        public const string Penalties = "App.Penalties";

        public const string TripCancelation = "App.Penalties.TripCancelation";
        public const string TripCancelationCommissionType = "App.Penalties.TripCancelation.CommissionType";
        public const string TripCancelationCommissionMinValue = "App.Penalties.TripCancelation.CommissionMinValue";
        public const string TripCancelationCommissionValue = "App.Penalties.TripCancelation.CommissionValue";
        public const string TripCancelationCommissionPercentage = "App.Penalties.TripCancelation.TripCancelationCommissionPercentage";

   
        public const string NotAssignTruckAndDriverStartDate = "App.Penalties.NotAssignTruckAndDriverStartDate";
        public const string NotAssignTruckAndDriverStartDate_NumberOfUnitsOfMeasure = "App.Penalties.NotAssignTruckAndDriverStartDate.NumberOfUnitsOfMeasure";
        public const string NotAssignTruckAndDriverStartDate_UnitsOfMeasure = "App.Penalties.NotAssignTruckAndDriverStartDate.UnitsOfMeasure";
        public const string NotAssignTruckAndDriverStartDate_Amount = "App.Penalties.NotAssignTruckAndDriverStartDate.Amount";
        public const string NotAssignTruckAndDriverStartDate_StartingAmount = "App.Penalties.NotAssignTruckAndDriverStartDate.StartingAmount";
        public const string NotAssignTruckAndDriverStartDate_MaximumAmount = "App.Penalties.NotAssignTruckAndDriverStartDate.MaximumAmount";
        public const string NotAssignTruckAndDriverStartDate_CommissionType = "App.Penalties.NotAssignTruckAndDriverStartDate.CommissionType";
        public const string NotAssignTruckAndDriverStartDate_CommissionMinValue = "App.Penalties.NotAssignTruckAndDriverStartDate.CommissionMinValue";
        public const string NotAssignTruckAndDriverStartDate_CommissionValue = "App.Penalties.NotAssignTruckAndDriverStartDate.CommissionValue";
        public const string NotAssignTruckAndDriverStartDate_CommissionPercentage = "App.Penalties.NotAssignTruckAndDriverStartDate.CommissionPercentage";

        public const string NotDeliveringAllDropsBeforeEndDate = "App.Penalties.NotDeliveringAllDropsBeforeEndDate";
        public const string NotDeliveringAllDropsBeforeEndDate_NumberOfUnitsOfMeasure = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.NumberOfUnitsOfMeasure";
        public const string NotDeliveringAllDropsBeforeEndDate_UnitsOfMeasure = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.UnitsOfMeasure";
        public const string NotDeliveringAllDropsBeforeEndDate_Amount = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.Amount";
        public const string NotDeliveringAllDropsBeforeEndDate_StartingAmount = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.StartingAmount";
        public const string NotDeliveringAllDropsBeforeEndDate_MaximumAmount = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.MaximumAmount";
        public const string NotDeliveringAllDropsBeforeEndDate_CommissionType = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.CommissionType";
        public const string NotDeliveringAllDropsBeforeEndDate_CommissionMinValue = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.CommissionMinValue";
        public const string NotDeliveringAllDropsBeforeEndDate_CommissionValue = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.CommissionValue";
        public const string NotDeliveringAllDropsBeforeEndDate_CommissionPercentage = "App.Penalties.NotDeliveringAllDropsBeforeEndDate.CommissionPercentage";



        public const string Detention = "App.Penalties.Detention";
        public const string MaxDetentionFeesAmount = "App.Penalties.Detention.MaxDetentionFeesAmount";
        public const string BaseDetentionFeesAmount = "App.Penalties.Detention.BaseDetentionFeesAmount";
        public const string DetentionFeesIncreaseRate = "App.Penalties.Detention.DetentionFeesIncreaseRate";
        public const string AllowedDetentionPeriod = "App.Penalties.Detention.AllowedDetentionPeriod";
        public const string DetentionCommissionType = "App.Penalties.Detention.CommissionType";
        public const string DetentionCommissionMinValue = "App.Penalties.Detention.CommissionMinValue";
        public const string DetentionCommissionValue = "App.Penalties.Detention.CommissionValue";
        public const string DetentionCommissionPercentage = "App.Penalties.Detention.CommissionPercentage";

    }
}