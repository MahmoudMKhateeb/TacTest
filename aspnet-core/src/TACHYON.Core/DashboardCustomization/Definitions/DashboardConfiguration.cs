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

            var tenantWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.Pages_Tenant_Dashboard
            };

           
            #endregion

            #region HostWidgets

            var hostWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.Pages_Administration_Host_Dashboard
            };

           

            #endregion

            #region ShipperWidgets


            var shipperWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.App_Shipper
            };


            var shipperNumberOfCompletedTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNumberOfCompletedTripsWidget, "ShipperNumberOfCompletedTripsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperAcceptedVsRejectedRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperAcceptedVsRejectedRequestsWidget, "ShipperAcceptedVsRejectedRequestsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperCompletedTripsVsPodWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperCompletedTripsVsPodWidget, "ShipperCompletedTripsVsPodWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperInvoicesVsPaidInvoicesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperInvoicesVsPaidInvoicesWidget, "ShipperInvoicesVsPaidInvoicesWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperNextInvoiceFrequancyEndDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNextInvoiceFrequancyEndDateWidget, "ShipperNextInvoiceFrequancyEndDateWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperInvoiceDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperInvoiceDueDateInDaysWidget, "ShipperInvoiceDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperDocumentDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperDocumentDueDateInDaysWidget, "ShipperDocumentDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperMostWorkedWithCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperMostWorkedWithCarriersWidget, "ShipperMostWorkedWithCarriersWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperMostUsedOriginsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperMostUsedOriginsWidget, "ShipperMostUsedOriginsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperMostUsedDestinationsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperMostUsedDestinationsWidget, "ShipperMostUsedDestinationsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperRequestsInMarketplaceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperRequestsInMarketplaceWidget, "ShipperRequestsInMarketplaceWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperTrackingMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperTrackingMapWidget, "ShipperTrackingMapWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);


            WidgetDefinitions.Add(shipperNumberOfCompletedTripsWidget);
            WidgetDefinitions.Add(shipperAcceptedVsRejectedRequestsWidget);
            WidgetDefinitions.Add(shipperCompletedTripsVsPodWidget);
            WidgetDefinitions.Add(shipperInvoicesVsPaidInvoicesWidget);
            WidgetDefinitions.Add(shipperNextInvoiceFrequancyEndDateWidget);
            WidgetDefinitions.Add(shipperInvoiceDueDateInDaysWidget);
            WidgetDefinitions.Add(shipperDocumentDueDateInDaysWidget);
            WidgetDefinitions.Add(shipperMostWorkedWithCarriersWidget);
            WidgetDefinitions.Add(shipperMostUsedOriginsWidget);
            WidgetDefinitions.Add(shipperMostUsedDestinationsWidget);
            WidgetDefinitions.Add(shipperRequestsInMarketplaceWidget);
            WidgetDefinitions.Add(shipperTrackingMapWidget);


            #endregion

            #region CarrierWidgets
            var carrierWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.App_Carrier
            };


            var carrierDriversActivityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierDriversActivityWidget, "CarrierDriversActivityWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierTrucksActivityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierTrucksActivityWidget, "CarrierTrucksActivityWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierAcceptedVsRejectedPricingWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierAcceptedVsRejectedPricingWidget, "CarrierAcceptedVsRejectedPricingWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierInvoicesVsPaidInvoicesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierInvoicesVsPaidInvoicesWidget, "CarrierInvoicesVsPaidInvoicesWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierMostUsedPpWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedPpWidget, "CarrierMostUsedPpWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierMostUsedVasWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedVasWidget, "CarrierMostUsedVasWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget, "CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidge", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierMostWorkedWithShipperWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostWorkedWithShipperWidget, "CarrierMostWorkedWithShipperWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierNextInvoiceFrequenctEndDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierNextInvoiceFrequenctEndDateWidget, "CarrierNextInvoiceFrequenctEndDateWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierDueDateInDaysWidget, "CarrierDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierTrackingMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierTrackingMapWidget, "CarrierTrackingMapWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);


            WidgetDefinitions.Add(carrierDriversActivityWidget);
            WidgetDefinitions.Add(carrierTrucksActivityWidget);
            WidgetDefinitions.Add(carrierAcceptedVsRejectedPricingWidget);
            WidgetDefinitions.Add(carrierInvoicesVsPaidInvoicesWidget);
            WidgetDefinitions.Add(carrierMostUsedPpWidget);
            WidgetDefinitions.Add(carrierMostUsedVasWidget);
            WidgetDefinitions.Add(carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget);
            WidgetDefinitions.Add(carrierMostWorkedWithShipperWidget);
            WidgetDefinitions.Add(carrierNextInvoiceFrequenctEndDateWidget);
            WidgetDefinitions.Add(carrierDueDateInDaysWidget);
            WidgetDefinitions.Add(carrierTrackingMapWidget);


            #endregion

            #region TachyonDealerWidgets

            var tachyonDealerNumberOfRegisteredTrucksWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredTrucksWidget, ("NumberOfRegisteredTrucksWidget"), side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNumberOfRegisteredShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredShippersWidget, "NumberOfRegisteredShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNumberOfRegisteredCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredCarriersWidget, "NumberOfRegisteredCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNewAccountsRegisteredWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NewAccountsRegisteredWidget, "NewAccountsRegisteredWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNewTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NewTripsWidget, "NewTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNumberOfDeliveredTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfDeliveredTripsWidget, "NumberOfDeliveredTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNumberOfOngoingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfOngoingTripsWidget, "NumberOfOngoingTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerTruckTypeUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TruckTypeUsageWidget, "TruckTypeUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerGoodTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.GoodTypesUsageWidget, "GoodTypesUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerRouteTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RouteTypesUsageWidget, "RouteTypesUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerMostRequestingShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.MostRequestingShippersWidget, "MostRequestingShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerMostRequestedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.MostRequestedCarriersWidget, "MostRequestedCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerTopRatedShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TopRatedShippersWidget, "TopRatedShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerTopRatedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TopRatedCarriersWidget, "TopRatedCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerWorstRatedShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.WorstRatedShippersWidget, "WorstRatedShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerWorstRatedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.WorstRatedCarriersWidget, "WorstRatedCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerUnPricedRequestsInMarketPlaceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.UnPricedRequestsInMarketPlaceWidget, "UnPricedRequestsInMarketPlaceWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerRequestsPricingBeforeBidEndingWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RequestsPricingBeforeBidEndingWidget, "RequestsPricingBeforeBidEndingWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerRequestsPriceAcceptanceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RequestsPriceAcceptanceWidget, "RequestsPriceAcceptanceWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerInvoicesPaidBeforeDueDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.InvoicesPaidBeforeDueDateWidget, "InvoicesPaidBeforeDueDateWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var tachyonDealerNumberOfRequestsPerAreaOrCityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRequestsPerAreaOrCityWidget, "NumberOfRequestsPerAreaOrCityWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            //var tachyonDealerSearchableMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TachyonDealerSearchableMapWidget, "TachyonDealerSearchableMapWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            //var tachyonDealerRequestsHeatMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TachyonDealerRequestsHeatMapWidget, "TachyonDealerRequestsHeatMapWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            //var tachyonDealerNormalVsRentalRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TachyonDealerNormalVsRentalRequestsWidget, "TachyonDealerNormalVsRentalRequestsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);


            WidgetDefinitions.Add(tachyonDealerNumberOfRegisteredTrucksWidget);
            WidgetDefinitions.Add(tachyonDealerNumberOfRegisteredShippersWidget);
            WidgetDefinitions.Add(tachyonDealerNumberOfRegisteredCarriersWidget);
            WidgetDefinitions.Add(tachyonDealerNewAccountsRegisteredWidget);
            WidgetDefinitions.Add(tachyonDealerNewTripsWidget);
            WidgetDefinitions.Add(tachyonDealerNumberOfDeliveredTripsWidget);
            WidgetDefinitions.Add(tachyonDealerNumberOfOngoingTripsWidget);
            WidgetDefinitions.Add(tachyonDealerTruckTypeUsageWidget);
            WidgetDefinitions.Add(tachyonDealerGoodTypesUsageWidget);
            WidgetDefinitions.Add(tachyonDealerRouteTypesUsageWidget);
            WidgetDefinitions.Add(tachyonDealerMostRequestingShippersWidget);
            WidgetDefinitions.Add(tachyonDealerMostRequestedCarriersWidget);
            WidgetDefinitions.Add(tachyonDealerTopRatedShippersWidget);
            WidgetDefinitions.Add(tachyonDealerTopRatedCarriersWidget);
            WidgetDefinitions.Add(tachyonDealerWorstRatedShippersWidget);
            WidgetDefinitions.Add(tachyonDealerWorstRatedCarriersWidget);
            WidgetDefinitions.Add(tachyonDealerUnPricedRequestsInMarketPlaceWidget);
            WidgetDefinitions.Add(tachyonDealerRequestsPricingBeforeBidEndingWidget);
            WidgetDefinitions.Add(tachyonDealerRequestsPriceAcceptanceWidget);
            WidgetDefinitions.Add(tachyonDealerInvoicesPaidBeforeDueDateWidget);
            WidgetDefinitions.Add(tachyonDealerNumberOfRequestsPerAreaOrCityWidget);
            //WidgetDefinitions.Add(tachyonDealerSearchableMapWidget);
            //WidgetDefinitions.Add(tachyonDealerRequestsHeatMapWidget);
            //WidgetDefinitions.Add(tachyonDealerNormalVsRentalRequestsWidget);


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
                    carrierMostUsedPpWidget.Id,
                    carrierMostUsedVasWidget.Id,
                    carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget.Id,
                    carrierMostWorkedWithShipperWidget.Id,
                    carrierNextInvoiceFrequenctEndDateWidget.Id,
                    carrierDueDateInDaysWidget.Id,
                    carrierTrackingMapWidget.Id,
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
                    shipperNextInvoiceFrequancyEndDateWidget.Id,
                    shipperInvoiceDueDateInDaysWidget.Id,
                    shipperDocumentDueDateInDaysWidget.Id,
                    shipperMostWorkedWithCarriersWidget.Id,
                    shipperMostUsedOriginsWidget.Id,
                    shipperMostUsedDestinationsWidget.Id,
                    shipperRequestsInMarketplaceWidget.Id,
                    shipperTrackingMapWidget.Id,
                });
            DashboardDefinitions.Add(defaultShipperDashboard);

            //tachyonMangedService
            var defaultTachyonMangedServiceDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultTachyonMangedServiceDashboard,
                new List<string>
                {
                    tachyonDealerNumberOfRegisteredTrucksWidget.Id,
                    tachyonDealerNumberOfRegisteredShippersWidget.Id,
                    tachyonDealerNumberOfRegisteredCarriersWidget.Id,
                    tachyonDealerNewAccountsRegisteredWidget.Id,
                    tachyonDealerNewTripsWidget.Id,
                    tachyonDealerNumberOfDeliveredTripsWidget.Id,
                    tachyonDealerNumberOfOngoingTripsWidget.Id,
                    tachyonDealerTruckTypeUsageWidget.Id,
                    tachyonDealerGoodTypesUsageWidget.Id,
                    tachyonDealerRouteTypesUsageWidget.Id,
                    tachyonDealerMostRequestingShippersWidget.Id,
                    tachyonDealerMostRequestedCarriersWidget.Id,
                    tachyonDealerTopRatedShippersWidget.Id,
                    tachyonDealerTopRatedCarriersWidget.Id,
                    tachyonDealerWorstRatedShippersWidget.Id,
                    tachyonDealerWorstRatedCarriersWidget.Id,
                    tachyonDealerUnPricedRequestsInMarketPlaceWidget.Id,
                    tachyonDealerRequestsPricingBeforeBidEndingWidget.Id,
                    tachyonDealerRequestsPriceAcceptanceWidget.Id,
                    tachyonDealerInvoicesPaidBeforeDueDateWidget.Id,
                    tachyonDealerNumberOfRequestsPerAreaOrCityWidget.Id,
                    //tachyonDealerSearchableMapWidget.Id,
                    //tachyonDealerRequestsHeatMapWidget.Id,
                    //tachyonDealerNormalVsRentalRequestsWidget.Id,
                });
            DashboardDefinitions.Add(defaultTachyonMangedServiceDashboard);


            // host

            var defaultHostDashboard = new DashboardDefinition(
                TACHYONDashboardCustomizationConsts.DashboardNames.DefaultHostDashboard,
                new List<string>
                {
                    // + Tachyon dealer widgets 
                    tachyonDealerNumberOfRegisteredTrucksWidget.Id,
                    tachyonDealerNumberOfRegisteredShippersWidget.Id,
                    tachyonDealerNumberOfRegisteredCarriersWidget.Id,
                    tachyonDealerNewAccountsRegisteredWidget.Id,
                    tachyonDealerNewTripsWidget.Id,
                    tachyonDealerNumberOfDeliveredTripsWidget.Id,
                    tachyonDealerNumberOfOngoingTripsWidget.Id,
                    tachyonDealerTruckTypeUsageWidget.Id,
                    tachyonDealerGoodTypesUsageWidget.Id,
                    tachyonDealerRouteTypesUsageWidget.Id,
                    tachyonDealerMostRequestingShippersWidget.Id,
                    tachyonDealerMostRequestedCarriersWidget.Id,
                    tachyonDealerTopRatedShippersWidget.Id,
                    tachyonDealerTopRatedCarriersWidget.Id,
                    tachyonDealerWorstRatedShippersWidget.Id,
                    tachyonDealerWorstRatedCarriersWidget.Id,
                    tachyonDealerUnPricedRequestsInMarketPlaceWidget.Id,
                    tachyonDealerRequestsPricingBeforeBidEndingWidget.Id,
                    tachyonDealerRequestsPriceAcceptanceWidget.Id,
                    tachyonDealerInvoicesPaidBeforeDueDateWidget.Id,
                    tachyonDealerNumberOfRequestsPerAreaOrCityWidget.Id,
                    //tachyonDealerSearchableMapWidget.Id,
                    //tachyonDealerRequestsHeatMapWidget.Id,
                    //tachyonDealerNormalVsRentalRequestsWidget.Id,
                });

            DashboardDefinitions.Add(defaultHostDashboard);

            // Add your dashboard definiton here

            #endregion

        }

    }
}