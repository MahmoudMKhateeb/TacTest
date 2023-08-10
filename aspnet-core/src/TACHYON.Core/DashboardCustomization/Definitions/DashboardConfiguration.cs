using Abp.MultiTenancy;
using System.Collections.Generic;
using System.Linq;
using TACHYON.Authorization;

namespace TACHYON.DashboardCustomization.Definitions
{
    public class DashboardConfiguration 
    {
        public List<DashboardDefinition> DashboardDefinitions { get; } = new List<DashboardDefinition>();

        public List<WidgetDefinition> WidgetDefinitions { get; } = new List<WidgetDefinition>();

        public List<WidgetFilterDefinition> WidgetFilterDefinitions { get; } = new List<WidgetFilterDefinition>();

        public DashboardConfiguration()
        {
            #region FilterDefinitions

            // These are global filter which all widgets can use
            var dateRangeFilter = new WidgetFilterDefinition(
                TACHYONDashboardCustomizationConsts.Filters.FilterDateRangePicker,
                "FilterDateRangePicker"
            );

            WidgetFilterDefinitions.Add(dateRangeFilter);

            // Add your filters here

            #endregion

            #region WidgetDefinitions

            // Define Widgets

            #region TenantWidgets

            var tenantWidgetsDefaultPermission = new List<string> { AppPermissions.Pages_Tenant_Dashboard };

           
            #endregion

            #region HostWidgets

            var hostWidgetsDefaultPermission = new List<string> { AppPermissions.Pages_Administration_Host_Dashboard };

            var hostNumberOfDeliveredTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfDeliveredTripsWidget, "NumberOfDeliveredTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfOngoingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfOngoingTripsWidget, "NumberOfOngoingTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostRouteTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.RouteTypesUsageWidget, "RouteTypesUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfRequestsPerAreaOrCityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfRequestsPerAreaOrCityWidget, "NumberOfRequestsPerAreaOrCityWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfRegisteredCompaniesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfRegisteredCompaniesWidget, "NumberOfRegisteredCompaniesWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfDriversAndTrucksWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfDriversAndTrucksWidget, "NumberOfDriversAndTrucksWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var TopRatedShippersAndCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.TopRatedShippersAndCarriersWidget, "TopRatedShippersAndCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NormalRequestsVSDedicatedRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NormalRequestsVSDedicatedRequestsWidget, "NormalRequestsVSDedicatedRequestsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var MostTruckTypeUsedWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.MostTruckTypeUsedWidget, "MostTruckTypeUsedWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfTripsWidget, "NumberOfTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfSaasTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfSaasTrips, "NumberOfSaasTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfTruckAggregationTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfTruckAggregationTrips, "NumberOfTruckAggregationTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfTruckAggregationTripsVsSaasTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfTruckAggregationTripsVsSaasTrips, "NumberOfTruckAggregationTripsVsSaasTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var TopWorstRatedPerTrip = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.TopWorstRatedPerTrip, "TopWorstRatedPerTrip", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var PaidInvoicesBeforeDueDate = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.PaidInvoicesBeforeDueDate, "PaidInvoicesBeforeDueDate", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var CostVsSellingVsProfitOfSaasTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.CostVsSellingVsProfitOfSaasTripsWidget, "CostVsSellingVsProfitOfSaasTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var OverallTotalAmountPerAllTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.OverallTotalAmountPerAllTrips, "OverallTotalAmountPerAllTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var TruckAggregationInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.TruckAggregationInvoices, "TruckAggregationInvoices", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var SaasInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.SaasInvoices, "SaasInvoices", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var GoodTypesUsage = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.GoodTypesUsage, "GoodTypesUsage", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var UpcomingTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.UpcomingTrips, "UpcomingTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NeedsActions = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NeedsActions, "NeedsActions", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var CostVsSellingVsProfitOfTruckAggregationTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.CostVsSellingVsProfitOfTruckAggregationTripsWidget, "CostVsSellingVsProfitOfTruckAggregationTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NewRegisteredCompanies = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NewRegisteredCompanies, "NewRegisteredCompanies", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var InvoicesVsPaidInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.InvoicesVsPaidInvoices, "InvoicesVsPaidInvoices", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var ClaimedInvoicesVsPaidInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.ClaimedInvoicesVsPaidInvoices, "ClaimedInvoicesVsPaidInvoices", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var NumberOfDedicatedTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfDedicatedTrips, "NumberOfDedicatedTrips", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);


            WidgetDefinitions.Add(hostNumberOfDeliveredTripsWidget);
            WidgetDefinitions.Add(hostNumberOfOngoingTripsWidget);
            WidgetDefinitions.Add(hostRouteTypesUsageWidget);
            WidgetDefinitions.Add(hostNumberOfRequestsPerAreaOrCityWidget);
            WidgetDefinitions.Add(NumberOfRegisteredCompaniesWidget);
            WidgetDefinitions.Add(NumberOfDriversAndTrucksWidget);
            WidgetDefinitions.Add(TopRatedShippersAndCarriersWidget);
            WidgetDefinitions.Add(NormalRequestsVSDedicatedRequestsWidget);
            WidgetDefinitions.Add(MostTruckTypeUsedWidget);
            WidgetDefinitions.Add(NumberOfTripsWidget);
            WidgetDefinitions.Add(NumberOfSaasTrips);
            WidgetDefinitions.Add(NumberOfTruckAggregationTrips);
            WidgetDefinitions.Add(NumberOfTruckAggregationTripsVsSaasTrips);
            WidgetDefinitions.Add(TopWorstRatedPerTrip);
            WidgetDefinitions.Add(PaidInvoicesBeforeDueDate);
            WidgetDefinitions.Add(CostVsSellingVsProfitOfSaasTripsWidget);
            WidgetDefinitions.Add(OverallTotalAmountPerAllTrips);
            WidgetDefinitions.Add(TruckAggregationInvoices);
            WidgetDefinitions.Add(SaasInvoices);
            WidgetDefinitions.Add(GoodTypesUsage);
            WidgetDefinitions.Add(UpcomingTrips);
            WidgetDefinitions.Add(NeedsActions);
            WidgetDefinitions.Add(CostVsSellingVsProfitOfTruckAggregationTrips);
            WidgetDefinitions.Add(NewRegisteredCompanies);
            WidgetDefinitions.Add(InvoicesVsPaidInvoices);
            WidgetDefinitions.Add(ClaimedInvoicesVsPaidInvoices);
            WidgetDefinitions.Add(NumberOfDedicatedTrips);


            #endregion

            #region ShipperWidgets

            var shipperWidgetsDefaultPermission = new List<string> { AppPermissions.Pages_ShipperDashboard, AppPermissions.Pages_ShipperDashboard_tripDetails };
            var shipperWidgetsDefaultPermissionWithTrackingMapOnly = new List<string> { AppPermissions.Pages_ShipperDashboard, AppPermissions.Pages_ShipperDashboard_trackingMap };

           

            var shipperNumberOfCompletedTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNumberOfCompletedTripsWidget, "ShipperNumberOfCompletedTripsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperAcceptedVsRejectedRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperAcceptedVsRejectedRequestsWidget, "ShipperAcceptedVsRejectedRequestsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperCompletedTripsVsPodWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperCompletedTripsVsPodWidget, "ShipperCompletedTripsVsPodWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperInvoicesVsPaidInvoicesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperInvoicesVsPaidInvoicesWidget, "ShipperInvoicesVsPaidInvoicesWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            //TODO as soon as
            var shipperInvoiceDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperInvoiceDueDateInDaysWidget, "ShipperInvoiceDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperDocumentDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperDocumentDueDateInDaysWidget, "ShipperDocumentDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperMostUsedOriginsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperMostUsedOriginsWidget, "ShipperMostUsedOriginsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperMostUsedDestinationsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperMostUsedDestinationsWidget, "ShipperMostUsedDestinationsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperRequestsInMarketplaceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperRequestsInMarketplaceWidget, "ShipperRequestsInMarketplaceWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperTrackingMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperTrackingMapWidget, "ShipperTrackingMapWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermissionWithTrackingMapOnly);
            var shipperCountersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperCountersWidget, "ShipperCountersWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperUpcomingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperUpcomingTripsWidget, "ShipperUpcomingTripsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperNeedsActionWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNeedsActionWidget, "ShipperNeedsActionWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperNewOffersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNewOffersWidget, "ShipperNewOffersWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);

            WidgetDefinitions.Add(shipperAcceptedVsRejectedRequestsWidget);
            WidgetDefinitions.Add(shipperCompletedTripsVsPodWidget);
            WidgetDefinitions.Add(shipperInvoicesVsPaidInvoicesWidget);
            WidgetDefinitions.Add(shipperMostUsedOriginsWidget);
            WidgetDefinitions.Add(shipperMostUsedDestinationsWidget);
            WidgetDefinitions.Add(shipperTrackingMapWidget);
            WidgetDefinitions.Add(shipperCountersWidget);
            WidgetDefinitions.Add(shipperUpcomingTripsWidget);
            WidgetDefinitions.Add(shipperNeedsActionWidget);
            WidgetDefinitions.Add(shipperNewOffersWidget);

            #endregion

            #region CarrierWidgets

            var carrierWidgetsDefaultPermission = new List<string> { AppPermissions.Pages_CarrierDashboard };

            var carrierDriversActivityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierDriversActivityWidget, "CarrierDriversActivityWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierTrucksActivityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierTrucksActivityWidget, "CarrierTrucksActivityWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierAcceptedVsRejectedPricingWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierAcceptedVsRejectedPricingWidget, "CarrierAcceptedVsRejectedPricingWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierInvoicesVsPaidInvoicesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierInvoicesVsPaidInvoicesWidget, "CarrierInvoicesVsPaidInvoicesWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierMostUsedVasWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedVasWidget, "CarrierMostUsedVasWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget, "CarrierNumberOfCompletedTripsWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierDueDateInDaysWidget, "CarrierDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierTrackingMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierTrackingMapWidget, "CarrierTrackingMapWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierMostUsedPPWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedPPWidget, "CarrierMostUsedPPWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierActiveDriversAndTrucksWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierActiveDriversAndTrucksWidget, "CarrierActiveDriversAndTrucksWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierUpcomingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierUpcomingTripsWidget, "CarrierUpcomingTripsWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierCountersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierCountersWidget, "CarrierCountersWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierNeedsActionWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.NeedsActionWidget, "NeedsActionWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierNewDirectRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.NewDirectRequestsWidget, "NewDirectRequestsWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);





            WidgetDefinitions.Add(carrierAcceptedVsRejectedPricingWidget);
            WidgetDefinitions.Add(carrierInvoicesVsPaidInvoicesWidget);
            WidgetDefinitions.Add(carrierMostUsedVasWidget);
            WidgetDefinitions.Add(carrierTrackingMapWidget);
            WidgetDefinitions.Add(CarrierMostUsedPPWidget);
            WidgetDefinitions.Add(CarrierActiveDriversAndTrucksWidget);
            WidgetDefinitions.Add(CarrierUpcomingTripsWidget);
            WidgetDefinitions.Add(CarrierCountersWidget);
            WidgetDefinitions.Add(CarrierNeedsActionWidget);
            WidgetDefinitions.Add(CarrierNewDirectRequestsWidget);

            #endregion

            #region TachyonDealerWidgets


            var tachyonDealerWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.App_TachyonDealer
            };

            var tachyonDealerNumberOfDeliveredTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfDeliveredTripsWidget, "NumberOfDeliveredTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfOngoingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfOngoingTripsWidget, "TMSNumberOfOngoingTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerRouteTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RouteTypesUsageWidget, "RouteTypesUsageWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfRequestsPerAreaOrCityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRequestsPerAreaOrCityWidget, "NumberOfRequestsPerAreaOrCityWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfRegisteredCompaniesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredCompaniesWidget, "NumberOfRegisteredCompaniesWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfDriversAndTrucksWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfDriversAndTrucksWidget, "NumberOfDriversAndTrucksWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerTopRatedShippersAndCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TopRatedShippersAndCarriersWidget, "TopRatedShippersAndCarriersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNormalRequestsVSDedicatedRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NormalRequestsVSDedicatedRequestsWidget, "NormalRequestsVSDedicatedRequestsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerMostTruckTypeUsedWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.MostTruckTypeUsedWidget, "MostTruckTypeUsedWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfTripsWidget, "NumberOfTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfSaasTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfSaasTrips, "NumberOfSaasTrips", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfTruckAggregationTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfTruckAggregationTrips, "NumberOfTruckAggregationTrips", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfTruckAggregationTripsVsSaasTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfTruckAggregationTripsVsSaasTrips, "NumberOfTruckAggregationTripsVsSaasTrips", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerTopWorstRatedPerTrip = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TopWorstRatedPerTrip, "TopWorstRatedPerTrip", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerPaidInvoicesBeforeDueDate = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.PaidInvoicesBeforeDueDate, "PaidInvoicesBeforeDueDate", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerCostVsSellingVsProfitOfSaasTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.CostVsSellingVsProfitOfSaasTripsWidget, "CostVsSellingVsProfitOfSaasTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerOverallTotalAmountPerAllTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.OverallTotalAmountPerAllTrips, "OverallTotalAmountPerAllTrips", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerTruckAggregationInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TruckAggregationInvoices, "TruckAggregationInvoices", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerSaasInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.SaasInvoices, "SaasInvoices", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerGoodTypesUsage = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.GoodTypesUsage, "GoodTypesUsage", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerUpcomingTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.UpcomingTrips, "UpcomingTrips", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNeedsActions = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NeedsActions, "NeedsActions", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerCostVsSellingVsProfitOfTruckAggregationTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.CostVsSellingVsProfitOfTruckAggregationTripsWidget, "CostVsSellingVsProfitOfTruckAggregationTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNewRegisteredCompanies = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NewRegisteredCompanies, "NewRegisteredCompanies", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerInvoicesVsPaidInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.InvoicesVsPaidInvoices, "InvoicesVsPaidInvoices", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerClaimedInvoicesVsPaidInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.ClaimedInvoicesVsPaidInvoices, "ClaimedInvoicesVsPaidInvoices", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfDedicatedTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfDedicatedTrips, "ClaimedInvoicesVsPaidInvoices", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);

              WidgetDefinitions.Add(tachyonDealerNumberOfDeliveredTripsWidget);  
              WidgetDefinitions.Add(tachyonDealerNumberOfOngoingTripsWidget);
              WidgetDefinitions.Add(tachyonDealerRouteTypesUsageWidget);
              WidgetDefinitions.Add(tachyonDealerNumberOfRequestsPerAreaOrCityWidget);   
              WidgetDefinitions.Add(tachyonDealerNumberOfRegisteredCompaniesWidget); 
              WidgetDefinitions.Add(tachyonDealerNumberOfDriversAndTrucksWidget);
              WidgetDefinitions.Add(tachyonDealerTopRatedShippersAndCarriersWidget); 
              WidgetDefinitions.Add(tachyonDealerNormalRequestsVSDedicatedRequestsWidget);   
              WidgetDefinitions.Add(tachyonDealerMostTruckTypeUsedWidget);   
              WidgetDefinitions.Add(tachyonDealerNumberOfTripsWidget);   
              WidgetDefinitions.Add(tachyonDealerNumberOfSaasTrips); 
              WidgetDefinitions.Add(tachyonDealerNumberOfTruckAggregationTrips); 
              WidgetDefinitions.Add(tachyonDealerNumberOfTruckAggregationTripsVsSaasTrips);  
              WidgetDefinitions.Add(tachyonDealerTopWorstRatedPerTrip);  
              WidgetDefinitions.Add(tachyonDealerPaidInvoicesBeforeDueDate); 
              WidgetDefinitions.Add(tachyonDealerCostVsSellingVsProfitOfSaasTripsWidget);
              WidgetDefinitions.Add(tachyonDealerOverallTotalAmountPerAllTrips); 
              WidgetDefinitions.Add(tachyonDealerTruckAggregationInvoices);  
              WidgetDefinitions.Add(tachyonDealerSaasInvoices);  
              WidgetDefinitions.Add(tachyonDealerGoodTypesUsage);
              WidgetDefinitions.Add(tachyonDealerUpcomingTrips); 
              WidgetDefinitions.Add(tachyonDealerNeedsActions);  
              WidgetDefinitions.Add(tachyonDealerCostVsSellingVsProfitOfTruckAggregationTrips);  
              WidgetDefinitions.Add(tachyonDealerNewRegisteredCompanies);
              WidgetDefinitions.Add(tachyonDealerInvoicesVsPaidInvoices);
              WidgetDefinitions.Add(tachyonDealerClaimedInvoicesVsPaidInvoices); 
              WidgetDefinitions.Add(tachyonDealerNumberOfDedicatedTrips); 

            #endregion

            #region BrokerWidgets

              // todo: add broker permission for widgets
              var brokerWidgetsDefaultPermission = new List<string>();

              var NewActorsThisMonthWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.NewActorsThisMonthWidget, "NewActorsThisMonthWidget", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var NumberOfActors = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.NumberOfActors, "NumberOfActors", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var MostActiveActorShipper = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.MostActiveActorShipper, "MostActiveActorShipper", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var MostActiveActorCarrier = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.MostActiveActorCarrier, "MostActiveActorCarrier", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var NumberOfActiveActors = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.NumberOfActiveActors, "NumberOfActiveActors", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var MostTruckTypeUsed = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.MostTruckTypeUsed, "MostTruckTypeUsed", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorNextDocDueDate = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorNextDocDueDate, "ActorNextDocDueDate", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorNextInvoiceDueDate = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorNextInvoiceDueDate, "ActorNextInvoiceDueDate", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var NewInvoicesVsPaidInvoices = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.NewInvoicesVsPaidInvoices, "NewInvoicesVsPaidInvoices", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var PendingPriceOffers = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.PendingPriceOffers, "PendingPriceOffers", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorsPaidInvoiceVsClaimedInvoice = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorsPaidInvoiceVsClaimedInvoice, "ActorsPaidInvoiceVsClaimedInvoice", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorsMostUsedOrigins = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorsMostUsedOrigins, "ActorsMostUsedOrigins", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorsMostUsedDestinations = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorsMostUsedDestinations, "ActorsMostUsedDestinations", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorsUpcomingTrips = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorsUpcomingTrips, "ActorsUpcomingTrips", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);
              var ActorsNeedsActions = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Broker.ActorsNeedsActions, "ActorsNeedsActions", side: MultiTenancySides.Tenant, permissions: brokerWidgetsDefaultPermission);





              WidgetDefinitions.Add(NewActorsThisMonthWidget);
              WidgetDefinitions.Add(NumberOfActors);
              WidgetDefinitions.Add(MostActiveActorShipper);
              WidgetDefinitions.Add(MostActiveActorCarrier);
              WidgetDefinitions.Add(NumberOfActiveActors);
              WidgetDefinitions.Add(MostTruckTypeUsed);
              WidgetDefinitions.Add(ActorNextDocDueDate);
              WidgetDefinitions.Add(ActorNextInvoiceDueDate);
              WidgetDefinitions.Add(NewInvoicesVsPaidInvoices);
              WidgetDefinitions.Add(PendingPriceOffers);
              WidgetDefinitions.Add(ActorsPaidInvoiceVsClaimedInvoice);
              WidgetDefinitions.Add(ActorsMostUsedOrigins);
              WidgetDefinitions.Add(ActorsMostUsedDestinations);
              WidgetDefinitions.Add(ActorsUpcomingTrips);
              WidgetDefinitions.Add(ActorsNeedsActions);

            #endregion

            #endregion

            #region DashboardDefinitions

           
            //carrier
            var defaultCarrierDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultCarrierDashboard,
                new List<string>
                {
                    carrierDriversActivityWidget.Id,
                    carrierTrucksActivityWidget.Id,
                    carrierAcceptedVsRejectedPricingWidget.Id,
                    carrierInvoicesVsPaidInvoicesWidget.Id,
                    carrierMostUsedVasWidget.Id,
                    carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget.Id,
                    carrierDueDateInDaysWidget.Id,
                    carrierTrackingMapWidget.Id,
                    CarrierMostUsedPPWidget.Id,
                    CarrierActiveDriversAndTrucksWidget.Id,
                    CarrierUpcomingTripsWidget.Id,
                    CarrierCountersWidget.Id,
                    CarrierNeedsActionWidget.Id,
                    CarrierNewDirectRequestsWidget.Id,
                    shipperCompletedTripsVsPodWidget.Id,
                });
            DashboardDefinitions.Add(defaultCarrierDashboard);

            //shipper
            var defaultShipperDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultShipperDashboard,
                new List<string>
                {
                    shipperNumberOfCompletedTripsWidget.Id,
                    shipperAcceptedVsRejectedRequestsWidget.Id,
                    shipperCompletedTripsVsPodWidget.Id,
                    shipperInvoicesVsPaidInvoicesWidget.Id,
                    shipperInvoiceDueDateInDaysWidget.Id,
                    shipperDocumentDueDateInDaysWidget.Id,
                    shipperMostUsedOriginsWidget.Id,
                    shipperMostUsedDestinationsWidget.Id,
                    shipperRequestsInMarketplaceWidget.Id,
                    shipperTrackingMapWidget.Id,
                    shipperCountersWidget.Id,
                    shipperUpcomingTripsWidget.Id,
                    shipperNeedsActionWidget.Id,
                    shipperNewOffersWidget.Id,
                });
            DashboardDefinitions.Add(defaultShipperDashboard);

            //tachyonMangedService
            var defaultTachyonMangedServiceDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultTachyonMangedServiceDashboard,
                new List<string>
                {
                    tachyonDealerNumberOfDeliveredTripsWidget.Id,
                    tachyonDealerNumberOfOngoingTripsWidget.Id,
                    tachyonDealerRouteTypesUsageWidget.Id,
                    tachyonDealerNumberOfRequestsPerAreaOrCityWidget.Id,
                    tachyonDealerNumberOfRegisteredCompaniesWidget.Id,
                    tachyonDealerNumberOfDriversAndTrucksWidget.Id,
                    tachyonDealerTopRatedShippersAndCarriersWidget.Id,
                    tachyonDealerNormalRequestsVSDedicatedRequestsWidget.Id,
                    tachyonDealerMostTruckTypeUsedWidget.Id,
                    tachyonDealerNumberOfTripsWidget.Id,
                    tachyonDealerNumberOfSaasTrips.Id,
                    tachyonDealerNumberOfTruckAggregationTrips.Id,
                    tachyonDealerNumberOfTruckAggregationTripsVsSaasTrips.Id,
                    tachyonDealerTopWorstRatedPerTrip.Id,
                    tachyonDealerPaidInvoicesBeforeDueDate.Id,
                    tachyonDealerCostVsSellingVsProfitOfSaasTripsWidget.Id,
                    tachyonDealerOverallTotalAmountPerAllTrips.Id,
                    tachyonDealerTruckAggregationInvoices.Id,
                    tachyonDealerSaasInvoices.Id,
                    tachyonDealerGoodTypesUsage.Id,
                    tachyonDealerUpcomingTrips.Id,
                    tachyonDealerNeedsActions.Id,
                    tachyonDealerCostVsSellingVsProfitOfTruckAggregationTrips.Id,
                    tachyonDealerNewRegisteredCompanies.Id,
                    tachyonDealerInvoicesVsPaidInvoices.Id,
                    tachyonDealerClaimedInvoicesVsPaidInvoices.Id,
                    tachyonDealerNumberOfDedicatedTrips.Id,
                });
            DashboardDefinitions.Add(defaultTachyonMangedServiceDashboard);


            // host

            var defaultHostDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultHostDashboard,
                new List<string>
                {
                    hostNumberOfDeliveredTripsWidget.Id,
                    hostNumberOfOngoingTripsWidget.Id,
                    hostRouteTypesUsageWidget.Id,
                    hostNumberOfRequestsPerAreaOrCityWidget.Id,
                    NumberOfRegisteredCompaniesWidget.Id,
                    NumberOfDriversAndTrucksWidget.Id,
                    TopRatedShippersAndCarriersWidget.Id,
                    NormalRequestsVSDedicatedRequestsWidget.Id,
                    MostTruckTypeUsedWidget.Id,
                    NumberOfTripsWidget.Id,
                    NumberOfSaasTrips.Id,
                    NumberOfTruckAggregationTrips.Id,
                    NumberOfTruckAggregationTripsVsSaasTrips.Id,
                    TopWorstRatedPerTrip.Id,
                    PaidInvoicesBeforeDueDate.Id,
                    CostVsSellingVsProfitOfSaasTripsWidget.Id,
                    OverallTotalAmountPerAllTrips.Id,
                    TruckAggregationInvoices.Id,
                    SaasInvoices.Id,
                    GoodTypesUsage.Id,
                    UpcomingTrips.Id,
                    NeedsActions.Id,
                    CostVsSellingVsProfitOfTruckAggregationTrips.Id,
                    NewRegisteredCompanies.Id,
                    InvoicesVsPaidInvoices.Id,
                    ClaimedInvoicesVsPaidInvoices.Id,
                    NumberOfDedicatedTrips.Id,
                });

            DashboardDefinitions.Add(defaultHostDashboard);

            //broker
            var defaultBrokerDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultBrokerDashboard,
                new List<string>
                {
                    shipperCompletedTripsVsPodWidget.Id,
                    shipperInvoicesVsPaidInvoicesWidget.Id,
                    shipperTrackingMapWidget.Id,
                    shipperCountersWidget.Id,
                    shipperUpcomingTripsWidget.Id,
                    shipperNeedsActionWidget.Id,
                    carrierMostUsedVasWidget.Id,
                    CarrierActiveDriversAndTrucksWidget.Id,
                    NewActorsThisMonthWidget.Id,
                    NumberOfActors.Id,
                    MostActiveActorShipper.Id,
                    MostActiveActorCarrier.Id,
                    NumberOfActiveActors.Id,
                    MostTruckTypeUsed.Id,
                    ActorNextDocDueDate.Id,
                    ActorNextInvoiceDueDate.Id,
                    NewInvoicesVsPaidInvoices.Id,
                    PendingPriceOffers.Id,
                    ActorsPaidInvoiceVsClaimedInvoice.Id,
                    ActorsMostUsedOrigins.Id,
                    ActorsMostUsedDestinations.Id,
                    ActorsUpcomingTrips.Id,
                    ActorsNeedsActions.Id,
                });
            DashboardDefinitions.Add(defaultBrokerDashboard);
            // Add your dashboard definiton here

            #endregion
        }
    }
}