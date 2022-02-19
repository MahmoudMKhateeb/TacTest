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
        public const string Broker = "App.Broker";
        public const string Receiver = "App.Receiver";
        public const string TachyonDealer = "App.TachyonDealer";
        public const string ShippingRequest = "App.shippingRequest";
        public const string MarketPlace = "App.MarketPlace";
        public const string OffersMarketPlace = "App.OffersMarketPlace";
        public const string SendDirectRequest = "App.SendDirectRequest";
        public const string SendTachyonDealShippingRequest = "App.SendTachyonDealShippingRequest";
        public const string CarrierAsASaas = "App.CarrierAsASaas";
        public const string CarrierAsSaasCommissionValue = "App.CarrierAsASaas.carrierAsSaasCommissionValue";
        public const string AddTripsByTachyonDeal = "App.Shipper.AddTripsByTachyonDeal";
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
        public const string TripCommissionPercentage = "App.Shipper.TripCommissionPercentage";
        public const string TripCommissionValue = "App.Shipper.TripCommissionValue";
        public const string TripMinValueCommission = "App.Shipper.TripMinValueCommission";
        public const string TripCommissionType = "App.Shipper.TripCommissionType";

        public static string DirectRequestCommissionPercentage = "App.Shipper.DirectRequestCommissionPercentage";
        public static string DirectRequestCommissionValue = "App.Shipper.DirectRequestCommissionValue";
        public static string DirectRequestCommissionType = "App.Shipper.DirectRequestCommissionType";
        public static string DirectRequestCommissionMinValue = "App.Shipper.DirectRequestCommissionMinValue";



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
        #endregion
        #region TachyonDealer
        #region Trip Comission
        public const string TachyonDealerTripCommissionPercentage = "App.TachyonDealer.TripCommissionPercentage";
        public const string TachyonDealerTripCommissionValue = "App.TachyonDealer.TripCommissionValue";
        public const string TachyonDealerTripMinValueCommission = "App.TachyonDealer.TripMinValueCommission";
        public const string TachyonDealerTripCommissionType = "App.TachyonDealer.TripCommissionType";
        #endregion
        #region Vas Comission
        public const string TachyonDealerVasCommissionPercentage = "App.TachyonDealer.VasCommissionPercentage";
        public const string TachyonDealerVasCommissionValue = "App.TachyonDealer.VasCommissionValue";
        public const string TachyonDealerVasMinValueCommission = "App.TachyonDealer.VasMinValueCommission";
        public const string TachyonDealerVasCommissionType = "App.TachyonDealer.VasCommissionType";
        #endregion
        #endregion
        #endregion
         public const string BayanIntegration = "App.BayanIntegration";
        public const string SaasRelatedCarrier = "App.Shipper.SaaSCarrier";
        public const string Saas = "App.Shipper.Saas";

        #endregion

        public const string Bidding = "App.Pay.Bidding";
        public const string CreateDirectRequest = "App.Pay.CreateDirectRequest";
        public const string CreateTmsRequest = "App.Pay.CreateTmsRequest";
        public const string PayPeriod = "App.Pay.PayPeriod";

    }
}