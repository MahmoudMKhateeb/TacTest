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

                public const string ShipperMostUsedOriginsWidget = "Widgets_Tenant_shipper_MostUsedOriginsWidget";

                public const string ShipperMostUsedDestinationsWidget =
                    "Widgets_Tenant_shipper_MostUsedDestinationsWidget";

                public const string ShipperRequestsInMarketplaceWidget =
                    "Widgets_Tenant_shipper_RequestsInMarketplaceWidget";

                public const string ShipperTrackingMapWidget = "Widgets_Tenant_shipper_TrackingMapWidget";
                public const string ShipperCountersWidget = "Widgets_Tenant_shipper_Counters_Widget";
                public const string ShipperUpcomingTripsWidget = "Widgets_Tenant_shipper_Upcoming_Trips_Widget";
                public const string ShipperNeedsActionWidget = "Widgets_Tenant_shipper_Needs_Action_Widget";
                public const string ShipperNewOffersWidget = "Widgets_Tenant_shipper_New_Offers_Widget";
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


                public const string CarrierNextInvoiceFrequenctEndDateWidget =
                    "Widgets_Tenant_carrier_NextInvoiceFrequenctEndDateWidget";

                public const string CarrierDueDateInDaysWidget = "Widgets_Tenant_carrier_DueDateInDaysWidget";
                public const string CarrierTrackingMapWidget = "Widgets_Tenant_carrier_TrackingMapWidget";
                public const string CarrierMostUsedPPWidget = "Widgets_Tenant_carrier_MostUsedPPWidget";
                public const string CarrierActiveDriversAndTrucksWidget = "Widgets_Tenant_carrier_active_drivers_and_trucks_Widget";
                public const string CarrierUpcomingTripsWidget = "Widgets_Tenant_carrier_Upcoming_Trips_Widget";
                public const string CarrierCountersWidget = "Widgets_Tenant_carrier_Counters_Widget";
                public const string NeedsActionWidget = "Widgets_Tenant_carrier_Needs_Action_Widget";
                public const string NewDirectRequestsWidget = "Widgets_Tenant_carrier_New_Direct_request";
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
                public const string NumberOfRegisteredCompaniesWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredCompaniesWidget";
                public const string NumberOfDriversAndTrucksWidget = "Widgets_Tenant_TachyonDealer_NumberOfDriversAndTrucksWidget";
                public const string TopRatedShippersAndCarriersWidget = "Widgets_Tenant_TachyonDealer_TopRatedShippersAndCarriersWidget";
                public const string NormalRequestsVSDedicatedRequestsWidget = "Widgets_Tenant_TachyonDealer_NormalRequestsVSDedicatedRequestsWidget";
                public const string MostTruckTypeUsedWidget = "Widgets_Tenant_TachyonDealer_MostTruckTypeUsedWidget";
                public const string NumberOfTripsWidget = "Widgets_Tenant_TachyonDealer_NumberOfTripsWidget";
                public const string NumberOfSaasTrips = "Widgets_Tenant_TachyonDealer_NumberOfSaasTripsWidget";
                public const string NumberOfTruckAggregationTrips = "Widgets_Tenant_TachyonDealer_NumberOfTruckAggregationTripsWidget";
                public const string NumberOfTruckAggregationTripsVsSaasTrips = "Widgets_Tenant_TachyonDealer_NumberOfTruckAggregationTripsVsSaasTripsWidget";
                public const string TopWorstRatedPerTrip = "Widgets_Tenant_TachyonDealer_TopWorstRatedPerTripComponentWidget";
                public const string PaidInvoicesBeforeDueDate = "Widgets_Tenant_TachyonDealer_PaidInvoicesBeforeDueDateWidget";
                public const string CostVsSellingVsProfitOfSaasTripsWidget = "Widgets_Tenant_TachyonDealer_CostVsSellingVsProfitOfSaasTripsWidget";
                public const string OverallTotalAmountPerAllTrips = "Widgets_Tenant_TachyonDealer_OverallTotalAmountPerAllTripsWidget";
                public const string TruckAggregationInvoices = "Widgets_Tenant_TachyonDealer_TruckAggregationInvoicesWidget";
                public const string SaasInvoices = "Widgets_Tenant_TachyonDealer_SaasInvoicesWidget";
                public const string GoodTypesUsage = "Widgets_Tenant_TachyonDealer_GoodTypesUsageWidget";
                public const string UpcomingTrips = "Widgets_Tenant_TachyonDealer_UpcomingTripsWidget";
                public const string NeedsActions = "Widgets_Tenant_TachyonDealer_NeedsActionsWidget";
                public const string CostVsSellingVsProfitOfTruckAggregationTripsWidget = "Widgets_Tenant_TachyonDealer_CostVsSellingVsProfitOfTruckAggregationTripsWidget";
                public const string NewRegisteredCompanies = "Widgets_Tenant_TachyonDealer_NewRegisteredCompanies";
                public const string InvoicesVsPaidInvoices = "Widgets_Tenant_TachyonDealer_InvoicesVsPaidInvoicesComponent";
                public const string ClaimedInvoicesVsPaidInvoices = "Widgets_Tenant_TachyonDealer_ClaimedInvoicesVsPaidInvoices";
            }

            public class Host
            {
                public const string NumberOfRegisteredTrucksWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredTrucksWidget";
                public const string NumberOfRegisteredShippersWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredShippersWidget";
                public const string NumberOfRegisteredCarriersWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredCarriersWidget";
                public const string NewAccountsRegisteredWidget = "Widgets_Tenant_TachyonDealer_NewAccountsRegisterdWidget";
                public const string NewTripsWidget = "Widgets_Tenant_TachyonDealer_NewTripsWidget";
                public const string NumberOfDeliveredTripsWidget = "Widgets_Tenant_TachyonDealer_NumberOfDeliverdTripsWidget";
                public const string NumberOfOngoingTripsWidget = "Widgets_Tenant_TachyonDealer_NumberOfOngoingTripsWidget";
                public const string TruckTypeUsageWidget = "Widgets_Tenant_TachyonDealer_TruckTypeUsageWidget";
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
                public const string NumberOfRegisteredCompaniesWidget = "Widgets_Tenant_TachyonDealer_NumberOfRegisteredCompaniesWidget";
                public const string NumberOfDriversAndTrucksWidget = "Widgets_Tenant_Host_NumberOfDriversAndTrucksWidget";
                public const string TopRatedShippersAndCarriersWidget = "Widgets_Tenant_Host_TopRatedShippersAndCarriersWidget";
                public const string NormalRequestsVSDedicatedRequestsWidget = "Widgets_Tenant_Host_NormalRequestsVSDedicatedRequestsWidget";
                public const string MostTruckTypeUsedWidget = "Widgets_Tenant_Host_MostTruckTypeUsedWidget";
                public const string NumberOfTripsWidget = "Widgets_Tenant_Host_NumberOfTripsWidget";
                public const string NumberOfSaasTrips = "Widgets_Tenant_Host_NumberOfSaasTripsWidget";
                public const string NumberOfTruckAggregationTrips = "Widgets_Tenant_Host_NumberOfTruckAggregationTripsWidget";
                public const string NumberOfTruckAggregationTripsVsSaasTrips = "Widgets_Tenant_Host_NumberOfTruckAggregationTripsVsSaasTripsWidget";
                public const string TopWorstRatedPerTrip = "Widgets_Tenant_Host_TopWorstRatedPerTripComponentWidget";
                public const string PaidInvoicesBeforeDueDate = "Widgets_Tenant_Host_PaidInvoicesBeforeDueDateWidget";
                public const string CostVsSellingVsProfitOfSaasTripsWidget = "Widgets_Tenant_Host_CostVsSellingVsProfitOfSaasTripsWidget";
                public const string OverallTotalAmountPerAllTrips = "Widgets_Tenant_Host_OverallTotalAmountPerAllTripsWidget";
                public const string TruckAggregationInvoices = "Widgets_Tenant_Host_TruckAggregationInvoicesWidget";
                public const string SaasInvoices = "Widgets_Tenant_Host_SaasInvoicesWidget";
                public const string GoodTypesUsage = "Widgets_Tenant_Host_GoodTypesUsageWidget";
                public const string UpcomingTrips = "Widgets_Tenant_Host_UpcomingTripsWidget";
                public const string NeedsActions = "Widgets_Tenant_Host_NeedsActionsWidget";
                public const string CostVsSellingVsProfitOfTruckAggregationTripsWidget = "Widgets_Tenant_Host_CostVsSellingVsProfitOfTruckAggregationTripsWidget";
                public const string NewRegisteredCompanies = "Widgets_Tenant_Host_NewRegisteredCompanies";
                public const string InvoicesVsPaidInvoices = "Widgets_Tenant_Host_InvoicesVsPaidInvoicesComponent";
                public const string ClaimedInvoicesVsPaidInvoices = "Widgets_Tenant_Host_ClaimedInvoicesVsPaidInvoices";
            }
            
            public class Broker
            {
                public const string NewActorsThisMonthWidget = "Widgets_Tenant_Broker_New_Actors_This_Month";
                public const string NumberOfActors = "Widgets_Tenant_Broker_Number_Of_Actors";
                public const string MostActiveActorShipper = "Widgets_Tenant_Broker_Most_Active_Actor_Shipper";
                public const string MostActiveActorCarrier = "Widgets_Tenant_Broker_Most_Active_Actor_Carrier";
                public const string NumberOfActiveActors = "Widgets_Tenant_Broker_Number_Of_Active_Actors";
                public const string MostTruckTypeUsed = "Widgets_Tenant_Broker_Most_Truck_Type_Used";
                public const string ActorNextDocDueDate = "Widgets_Tenant_Broker_Actor_Next_Doc_Due_Date";
                public const string ActorNextInvoiceDueDate = "Widgets_Tenant_Broker_Actor_Next_Invoice_Due_Date";
                public const string NewInvoicesVsPaidInvoices = "Widgets_Tenant_Broker_New_Invoices_Vs_Paid_Invoices";
                public const string PendingPriceOffers = "Widgets_Tenant_Broker_Pending_Price_Offers";
                public const string ActorsPaidInvoiceVsClaimedInvoice = "Widgets_Tenant_Broker_Actors_Paid_Invoice_Vs_Claimed_Invoice";
                public const string ActorsMostUsedOrigins = "Widgets_Tenant_Broker_Actors_Most_Used_Origins";
                public const string ActorsMostUsedDestinations = "Widgets_Tenant_Broker_Actors_Most_Used_Destinations";
                public const string ActorsUpcomingTrips = "Widgets_Tenant_Broker_Actors_Upcoming_Trips";
                public const string ActorsNeedsActions = "Widgets_Tenant_Broker_Actors_Needs_Actions";
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
            public const string DefaultBrokerDashboard = "BrokerDashboard";
        }

        public class Applications
        {
            public const string Mvc = "Mvc";
            public const string Angular = "Angular";
        }
    }
}