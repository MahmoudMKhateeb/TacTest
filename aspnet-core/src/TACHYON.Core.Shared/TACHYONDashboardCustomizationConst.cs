namespace TACHYON
{
    public class TACHYONDashboardCustomizationConsts
    {
        /// <summary>
        /// Main page name your user will see if they dont change default page's name.
        /// </summary>
        public const string DefaultPageName = "Default Page";

        //Must use underscore instead of dot in widget and filter ids
        //(these data are also used as ids in the input in html pages. Please provide appropriate values.)
        public class Widgets
        {
            public class Tenant
            {
            }

            public class Shipper
            {
                public const string ShipperNumberOfCompletedTripsWidget = "Widgets_Tenant_shipper_NumberofcompletedTripsWidget";
                public const string ShipperAcceptedVsRejectedRequestsWidget = "Widgets_Tenant_shipper_AcceptedVsRejectedRequestsWidget";
                public const string ShipperCompletedTripsVsPodWidget = "Widgets_Tenant_shipper_CompletedTripsVsPodWidget";
                public const string ShipperInvoicesVsPaidInvoicesWidget = "Widgets_Tenant_shipper_InvoicesVsPaidInvoicesWidget";
                public const string ShipperNextInvoiceFrequancyEndDateWidget = "Widgets_Tenant_shipper_NextInvoiceFrequancyEndDateWidget";
                public const string ShipperInvoiceDueDateInDaysWidget = "Widgets_Tenant_shipper_InvoiceDueDateInDaysWidget";
                public const string ShipperDocumentDueDateInDaysWidget = "Widgets_Tenant_shipper_DocumentDueDateInDaysWidget";
                public const string ShipperMostWorkedWithCarriersWidget = "Widgets_Tenant_shipper_MostWorkedWithCarriersWidget";
                public const string ShipperMostUsedOriginsWidget = "Widgets_Tenant_shipper_MostUsedOriginsWidget";
                public const string ShipperMostUsedDestinationsWidget = "Widgets_Tenant_shipper_MostUsedDestinationsWidget";
                public const string ShipperRequestsInMarketplaceWidget = "Widgets_Tenant_shipper_RequestsInMarketplaceWidget";
                public const string ShipperTrackingMapWidget = "Widgets_Tenant_shipper_TrackingMapWidget";
            }
            public class Carrier
            {
                public const string CarrierDriversActivityWidget = "Widgets_Tenant_carrier_DriversActivityWidget";
                public const string CarrierTrucksActivityWidget = "Widgets_Tenant_carrier_TrucksActivityWidget";
                public const string CarrierAcceptedVsRejectedPricingWidget = "Widgets_Tenant_carrier_AcceptedVsRejectedPricingWidget";
                public const string CarrierInvoicesVsPaidInvoicesWidget = "Widgets_Tenant_carrier_InvoicesVsPaidInvoicesWidget";
                public const string CarrierMostUsedPpWidget = "Widgets_Tenant_carrier_MostUsedPPWidget";
                public const string CarrierMostUsedVasWidget = "Widgets_Tenant_carrier_MostUsedVasWidget";
                public const string CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget = "Widgets_Tenant_carrier_NumberOfCompletedTripsTotalMonthlyIncreaseWidget";
                public const string CarrierMostWorkedWithShipperWidget = "Widgets_Tenant_carrier_MostWorkedWithShipperWidget";
                public const string CarrierNextInvoiceFrequenctEndDateWidget = "Widgets_Tenant_carrier_NextInvoiceFrequenctEndDateWidget";
                public const string CarrierDueDateInDaysWidget = "Widgets_Tenant_carrier_DueDateInDaysWidget";
                public const string CarrierTrackingMapWidget = "Widgets_Tenant_carrier_TrackingMapWidget";
            }
            public class TachyonDealer
            {
                public const string NumberOfRegisteredTrucksWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredTrucksWidget";
                public const string NumberOfRegisteredShippersWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredShippersWidget";
                public const string NumberOfRegisteredCarriersWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredCarriersWidget";
                public const string NewAccountsRegisteredWidget = "Widgets_Tenant_TachyonDealer_NewAccountsRegisterdWidget";
                public const string NewTripsWidget = "Widgets_Tenant_TachyonDealer_NewTripsWidget";
                public const string NumberOfDeliveredTripsWidget = "Widgets_Tenant_TachyonDealer_NumberOfDeliverdTripsWidget";
                public const string NumberOfOngoingTripsWidget = "Widgets_Tenant_TachyonDealer_NumberOfOngoingTripsWidget";
                public const string TruckTypeUsageWidget = "Widgets_Tenant_TachyonDealer_TruckTypeUsageWidget";
                public const string GoodTypesUsageWidget = "Widgets_Tenant_TachyonDealer_GoodTypesUsageWidget";
                public const string RouteTypesUsageWidget = "Widgets_Tenant_TachyonDealer_RouteTypesUsageWidget";
                public const string MostRequestingShippersWidget = "Widgets_Tenant_TachyonDealer_MostRequestingShippersWidget";
                public const string MostRequestedCarriersWidget = "Widgets_Tenant_TachyonDealer_MostRequestedCarriersWidget";
                public const string TopRatedShippersWidget = "Widgets_Tenant_TachyonDealer_TopRatedShippersWidget";
                public const string TopRatedCarriersWidget = "Widgets_Tenant_TachyonDealer_TopRatedCarriersWidget";
                public const string WorstRatedShippersWidget = "Widgets_Tenant_TachyonDealer_WorstRatedShippersWidget";
                public const string WorstRatedCarriersWidget = "Widgets_Tenant_TachyonDealer_WorstRatedCarriersWidget";
                public const string UnPricedRequestsInMarketPlaceWidget = "Widgets_Tenant_TachyonDealer_UnpricedRequestsInMarketPlaceWidget";
                public const string RequestsPricingBeforeBidEndingWidget = "Widgets_Tenant_TachyonDealer_RequestsPricingBeforeBidEndingWidget";
                public const string RequestsPriceAcceptanceWidget = "Widgets_Tenant_TachyonDealer_RequestsPriceAcceptanceWidget";
                public const string InvoicesPaidBeforeDueDateWidget = "Widgets_Tenant_TachyonDealer_InvoicesPaidBeforeDureDateWidget";
                public const string NumberOfRequestsPerAreaOrCityWidget = "Widgets_Tenant_TachyonDealer_NumberOfRequestsPerAreaOrCityWidget";
                public const string SearchableMapWidget = "Widgets_Tenant_TachyonDealer_SearchableMapWidget";
                public const string RequestsHeatMapWidget = "Widgets_Tenant_TachyonDealer_RequestsHeatMapWidget";
                public const string NormalVsRentalRequestsWidget = "Widgets_Tenant_TachyonDealer_NormalVsRentalRequestsWidget";
            }
            public class Host
            {
            }


        }

        public class Filters
        {
            public const string FilterDateRangePicker = "Filters_DateRangePicker";
        }

        public class DashboardNames
        {
            public const string DefaultTenantDashboard = "TenantDashboard";
            public const string DefaultHostDashboard = "HostDashboard";


            public const string DefaultCarrierDashboard = "CarrierDashboard";
            public const string DefaultShipperDashboard = "ShipperDashboard";
            public const string DefaultTachyonMangedServiceDashboard = "TachyonMangedServiceDashboard";
        }

        public class Applications
        {
            public const string Mvc = "Mvc";
            public const string Angular = "Angular";
        }
    }
}