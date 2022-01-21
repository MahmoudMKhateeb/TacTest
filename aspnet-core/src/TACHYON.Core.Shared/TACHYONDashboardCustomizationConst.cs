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
                public const string GeneralStats = "Widgets_Tenant_GeneralStats";
                public const string DailySales = "Widgets_Tenant_DailySales";
                public const string ProfitShare = "Widgets_Tenant_ProfitShare";
                public const string MemberActivity = "Widgets_Tenant_MemberActivity";
                public const string RegionalStats = "Widgets_Tenant_RegionalStats";
                public const string SalesSummary = "Widgets_Tenant_SalesSummary";
                public const string TopStats = "Widgets_Tenant_TopStats";
            }

            public class Shipper
            {
                public const string ShipperNumberOfCompletedTripsWidget =
                    "Widgets_Tenant_shipper_NumberofcompletedTripsWidget";

                public const string ShipperAcceptedVsRejectedRequestsWidget =
                    "Widgets_Tenant_shipper_AcceptedVsRejectedRequestsWidget";

                public const string ShipperCompletedTripsVsPodWidget =
                    "Widgets_Tenant_shipper_CompletedTripsVsPodWidget";

                public const string ShipperInvoicesVsPaidInvoicesWidget =
                    "Widgets_Tenant_shipper_InvoicesVsPaidInvoicesWidget";

                public const string ShipperNextInvoiceFrequancyEndDateWidget =
                    "Widgets_Tenant_shipper_NextInvoiceFrequancyEndDateWidget";

                public const string ShipperInvoiceDueDateInDaysWidget =
                    "Widgets_Tenant_shipper_InvoiceDueDateInDaysWidget";

                public const string ShipperDocumentDueDateInDaysWidget =
                    "Widgets_Tenant_shipper_DocumentDueDateInDaysWidget";

                public const string ShipperMostWorkedWithCarriersWidget =
                    "Widgets_Tenant_shipper_MostWorkedWithCarriersWidget";

                public const string ShipperMostUsedOriginsWidget = "Widgets_Tenant_shipper_MostUsedOriginsWidget";

                public const string ShipperMostUsedDestinationsWidget =
                    "Widgets_Tenant_shipper_MostUsedDestinationsWidget";

                public const string ShipperRequestsInMarketplaceWidget =
                    "Widgets_Tenant_shipper_RequestsInMarketplaceWidget";

                public const string ShipperTrackingMapWidget = "Widgets_Tenant_shipper_TrackingMapWidget";
            }

            public class Carrier
            {
                public const string CarrierDriversActivityWidget = "Widgets_Tenant_carrier_DriversActivityWidget";
                public const string CarrierTrucksActivityWidget = "Widgets_Tenant_carrier_TrucksActivityWidget";

                public const string CarrierAcceptedVsRejectedPricingWidget =
                    "Widgets_Tenant_carrier_AcceptedVsRejectedPricingWidget";

                public const string CarrierInvoicesVsPaidInvoicesWidget =
                    "Widgets_Tenant_carrier_InvoicesVsPaidInvoicesWidget";

                public const string CarrierMostUsedPpWidget = "Widgets_Tenant_carrier_MostUsedPPWidget";
                public const string CarrierMostUsedVasWidget = "Widgets_Tenant_carrier_MostUsedVasWidget";

                public const string CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget =
                    "Widgets_Tenant_carrier_NumberOfCompletedTripsTotalMonthlyIncreaseWidget";

                public const string CarrierMostWorkedWithShipperWidget =
                    "Widgets_Tenant_carrier_MostWorkedWithShipperWidget";

                public const string CarrierNextInvoiceFrequenctEndDateWidget =
                    "Widgets_Tenant_carrier_NextInvoiceFrequenctEndDateWidget";

                public const string CarrierDueDateInDaysWidget = "Widgets_Tenant_carrier_DueDateInDaysWidget";
                public const string CarrierTrackingMapWidget = "Widgets_Tenant_carrier_TrackingMapWidget";
            }

            public class TachyonDealer
            {
                public const string TachyonDealerNumberOfRegisteredTrucksWidget =
                    "Widgets_Tenant_TachyonDealer_NumberOfRegisteredTrucksWidget";

                public const string TachyonDealerNumberOfRegisteredShippersWidget =
                    "Widgets_Tenant_TachyonDealer_NumberOfRegisteredShippersWidget";

                public const string TachyonDealerNumberOfRegisteredCarriersWidget =
                    "Widgets_Tenant_TachyonDealer_NumberOfRegisteredCarriersWidget";

                public const string TachyonDealerNewAccountsRegisteredWidget =
                    "Widgets_Tenant_TachyonDealer_NewAccountsRegisterdWidget";

                public const string TachyonDealerNewTripsWidget = "Widgets_Tenant_TachyonDealer_NewTripsWidget";

                public const string TachyonDealerNumberOfDeliveredTripsWidget =
                    "Widgets_Tenant_TachyonDealer_NumberOfDeliverdTripsWidget";

                public const string TachyonDealerNumberOfOngoingTripsWidget =
                    "Widgets_Tenant_TachyonDealer_NumberOfOngoingTripsWidget";

                public const string TachyonDealerTruckTypeUsageWidget =
                    "Widgets_Tenant_TachyonDealer_TruckTypeUsageWidget";

                public const string TachyonDealerGoodTypesUsageWidget =
                    "Widgets_Tenant_TachyonDealer_GoodTypesUsageWidget";

                public const string TachyonDealerRouteTypesUsageWidget =
                    "Widgets_Tenant_TachyonDealer_RouteTypesUsageWidget";

                public const string TachyonDealerMostRequestingShippersWidget =
                    "Widgets_Tenant_TachyonDealer_MostRequestingShippersWidget";

                public const string TachyonDealerMostRequestedCarriersWidget =
                    "Widgets_Tenant_TachyonDealer_MostRequestedCarriersWidget";

                public const string TachyonDealerTopRatedShippersWidget =
                    "Widgets_Tenant_TachyonDealer_TopRatedShippersWidget";

                public const string TachyonDealerTopRatedCarriersWidget =
                    "Widgets_Tenant_TachyonDealer_TopRatedCarriersWidget";

                public const string TachyonDealerWorstRatedShippersWidget =
                    "Widgets_Tenant_TachyonDealer_WorstRatedShippersWidget";

                public const string TachyonDealerWorstRatedCarriersWidget =
                    "Widgets_Tenant_TachyonDealer_WorstRatedCarriersWidget";

                public const string TachyonDealerUnPricedRequestsInMarketPlaceWidget =
                    "Widgets_Tenant_TachyonDealer_UnpricedRequestsInMarketPlaceWidget";

                public const string TachyonDealerRequestsPricingBeforeBidEndingWidget =
                    "Widgets_Tenant_TachyonDealer_RequestsPricingBeforeBidEndingWidget";

                public const string TachyonDealerRequestsPriceAcceptanceWidget =
                    "Widgets_Tenant_TachyonDealer_RequestsPriceAcceptanceWidget";

                public const string TachyonDealerInvoicesPaidBeforeDueDateWidget =
                    "Widgets_Tenant_TachyonDealer_InvoicesPaidBeforeDureDateWidget";

                public const string TachyonDealerNumberOfRequestsPerAreaOrCityWidget =
                    "Widgets_Tenant_TachyonDealer_NumberOfRequestsPerAreaOrCityWidget";

                public const string TachyonDealerSearchableMapWidget =
                    "Widgets_Tenant_TachyonDealer_SearchableMapWidget";

                public const string TachyonDealerRequestsHeatMapWidget =
                    "Widgets_Tenant_TachyonDealer_RequestsHeatMapWidget";

                public const string TachyonDealerNormalVsRentalRequestsWidget =
                    "Widgets_Tenant_TachyonDealer_NormalVsRentalRequestsWidget";
            }

            public class Host
            {
                public const string TopStats = "Widgets_Host_TopStats";
                public const string IncomeStatistics = "Widgets_Host_IncomeStatistics";
                public const string EditionStatistics = "Widgets_Host_EditionStatistics";
                public const string SubscriptionExpiringTenants = "Widgets_Host_SubscriptionExpiringTenants";
                public const string RecentTenants = "Widgets_Host_RecentTenants";
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