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

            var hostNumberOfRegisteredTrucksWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfRegisteredTrucksWidget, ("NumberOfRegisteredTrucksWidget"), side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfRegisteredShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfRegisteredShippersWidget, "NumberOfRegisteredShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfRegisteredCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfRegisteredCarriersWidget, "NumberOfRegisteredCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNewAccountsRegisteredWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NewAccountsRegisteredWidget, "NewAccountsRegisteredWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNewTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NewTripsWidget, "NewTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfDeliveredTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfDeliveredTripsWidget, "NumberOfDeliveredTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfOngoingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfOngoingTripsWidget, "NumberOfOngoingTripsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostTruckTypeUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.TruckTypeUsageWidget, "TruckTypeUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostGoodTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.GoodTypesUsageWidget, "GoodTypesUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostRouteTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.RouteTypesUsageWidget, "RouteTypesUsageWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostMostRequestingShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.MostRequestingShippersWidget, "MostRequestingShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostMostRequestedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.MostRequestedCarriersWidget, "MostRequestedCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostTopRatedShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.TopRatedShippersWidget, "TopRatedShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostTopRatedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.TopRatedCarriersWidget, "TopRatedCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostWorstRatedShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.WorstRatedShippersWidget, "WorstRatedShippersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostWorstRatedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.WorstRatedCarriersWidget, "WorstRatedCarriersWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostUnPricedRequestsInMarketPlaceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.UnPricedRequestsInMarketPlaceWidget, "UnPricedRequestsInMarketPlaceWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostRequestsPricingBeforeBidEndingWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.RequestsPricingBeforeBidEndingWidget, "RequestsPricingBeforeBidEndingWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostRequestsPriceAcceptanceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.RequestsPriceAcceptanceWidget, "RequestsPriceAcceptanceWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostInvoicesPaidBeforeDueDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.InvoicesPaidBeforeDueDateWidget, "InvoicesPaidBeforeDueDateWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            var hostNumberOfRequestsPerAreaOrCityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Host.NumberOfRequestsPerAreaOrCityWidget, "NumberOfRequestsPerAreaOrCityWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            //var tachyonDealerSearchableMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TachyonDealerSearchableMapWidget, "TachyonDealerSearchableMapWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            //var tachyonDealerRequestsHeatMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TachyonDealerRequestsHeatMapWidget, "TachyonDealerRequestsHeatMapWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);
            //var tachyonDealerNormalVsRentalRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TachyonDealerNormalVsRentalRequestsWidget, "TachyonDealerNormalVsRentalRequestsWidget", side: MultiTenancySides.Host, permissions: hostWidgetsDefaultPermission);


            WidgetDefinitions.Add(hostNumberOfRegisteredTrucksWidget);
            WidgetDefinitions.Add(hostNumberOfRegisteredShippersWidget);
            WidgetDefinitions.Add(hostNumberOfRegisteredCarriersWidget);
            WidgetDefinitions.Add(hostNewAccountsRegisteredWidget);
            WidgetDefinitions.Add(hostNewTripsWidget);
            WidgetDefinitions.Add(hostNumberOfDeliveredTripsWidget);
            WidgetDefinitions.Add(hostNumberOfOngoingTripsWidget);
            WidgetDefinitions.Add(hostTruckTypeUsageWidget);
            WidgetDefinitions.Add(hostGoodTypesUsageWidget);
            WidgetDefinitions.Add(hostRouteTypesUsageWidget);
            WidgetDefinitions.Add(hostMostRequestingShippersWidget);
            WidgetDefinitions.Add(hostMostRequestedCarriersWidget);
            WidgetDefinitions.Add(hostTopRatedShippersWidget);
            WidgetDefinitions.Add(hostTopRatedCarriersWidget);
            WidgetDefinitions.Add(hostWorstRatedShippersWidget);
            WidgetDefinitions.Add(hostWorstRatedCarriersWidget);
            WidgetDefinitions.Add(hostUnPricedRequestsInMarketPlaceWidget);
            WidgetDefinitions.Add(hostRequestsPricingBeforeBidEndingWidget);
            WidgetDefinitions.Add(hostRequestsPriceAcceptanceWidget);
            WidgetDefinitions.Add(hostInvoicesPaidBeforeDueDateWidget);
            WidgetDefinitions.Add(hostNumberOfRequestsPerAreaOrCityWidget);
            //WidgetDefinitions.Add(tachyonDealerSearchableMapWidget);
            //WidgetDefinitions.Add(tachyonDealerRequestsHeatMapWidget);
            //WidgetDefinitions.Add(tachyonDealerNormalVsRentalRequestsWidget);



            #endregion

            #region ShipperWidgets

            var shipperWidgetsDefaultPermission = new List<string> { AppPermissions.App_Shipper };


           

            var shipperNumberOfCompletedTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNumberOfCompletedTripsWidget, "ShipperNumberOfCompletedTripsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperAcceptedVsRejectedRequestsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperAcceptedVsRejectedRequestsWidget, "ShipperAcceptedVsRejectedRequestsWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperCompletedTripsVsPodWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperCompletedTripsVsPodWidget, "ShipperCompletedTripsVsPodWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            var shipperInvoicesVsPaidInvoicesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperInvoicesVsPaidInvoicesWidget, "ShipperInvoicesVsPaidInvoicesWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
            //TODO as soon as
            //var shipperNextInvoiceFrequancyEndDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Shipper.ShipperNextInvoiceFrequancyEndDateWidget, "ShipperNextInvoiceFrequancyEndDateWidget", side: MultiTenancySides.Tenant, permissions: shipperWidgetsDefaultPermission);
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
            //WidgetDefinitions.Add(shipperNextInvoiceFrequancyEndDateWidget);
            WidgetDefinitions.Add(shipperInvoiceDueDateInDaysWidget);
            WidgetDefinitions.Add(shipperDocumentDueDateInDaysWidget);
            WidgetDefinitions.Add(shipperMostWorkedWithCarriersWidget);
            WidgetDefinitions.Add(shipperMostUsedOriginsWidget);
            WidgetDefinitions.Add(shipperMostUsedDestinationsWidget);
            WidgetDefinitions.Add(shipperRequestsInMarketplaceWidget);
            WidgetDefinitions.Add(shipperTrackingMapWidget);

            #endregion

            #region CarrierWidgets

            var carrierWidgetsDefaultPermission = new List<string> { AppPermissions.App_Carrier };

            var carrierDriversActivityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierDriversActivityWidget, "CarrierDriversActivityWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierTrucksActivityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierTrucksActivityWidget, "CarrierTrucksActivityWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierAcceptedVsRejectedPricingWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierAcceptedVsRejectedPricingWidget, "CarrierAcceptedVsRejectedPricingWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierInvoicesVsPaidInvoicesWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierInvoicesVsPaidInvoicesWidget, "CarrierInvoicesVsPaidInvoicesWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            //var carrierMostUsedPpWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedPpWidget, "CarrierMostUsedPpWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierMostUsedVasWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedVasWidget, "CarrierMostUsedVasWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget, "CarrierNumberOfCompletedTripsWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierMostWorkedWithShipperWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostWorkedWithShipperWidget, "CarrierMostWorkedWithShipperWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            //var carrierNextInvoiceFrequenctEndDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierNextInvoiceFrequenctEndDateWidget, "CarrierNextInvoiceFrequenctEndDateWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierDueDateInDaysWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierDueDateInDaysWidget, "CarrierDueDateInDaysWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var carrierTrackingMapWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierTrackingMapWidget, "CarrierTrackingMapWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);
            var CarrierMostUsedPPWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.Carrier.CarrierMostUsedPPWidget, "CarrierMostUsedPPWidget", side: MultiTenancySides.Tenant, permissions: carrierWidgetsDefaultPermission);





            WidgetDefinitions.Add(carrierDriversActivityWidget);
            WidgetDefinitions.Add(carrierTrucksActivityWidget);
            WidgetDefinitions.Add(carrierAcceptedVsRejectedPricingWidget);
            WidgetDefinitions.Add(carrierInvoicesVsPaidInvoicesWidget);
            //WidgetDefinitions.Add(carrierMostUsedPpWidget);
            WidgetDefinitions.Add(carrierMostUsedVasWidget);
            WidgetDefinitions.Add(carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget);
            WidgetDefinitions.Add(carrierMostWorkedWithShipperWidget);
            //WidgetDefinitions.Add(carrierNextInvoiceFrequenctEndDateWidget);
            WidgetDefinitions.Add(carrierDueDateInDaysWidget);
            WidgetDefinitions.Add(carrierTrackingMapWidget);
            WidgetDefinitions.Add(CarrierMostUsedPPWidget);

            #endregion

            #region TachyonDealerWidgets


            var tachyonDealerWidgetsDefaultPermission = new List<string>
            {
                AppPermissions.App_TachyonDealer
            };

            var tachyonDealerNumberOfRegisteredTrucksWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredTrucksWidget, ("TMSNumberOfRegisteredTrucksWidget"), side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfRegisteredShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredShippersWidget, "TMSNumberOfRegisteredShippersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfRegisteredCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRegisteredCarriersWidget, "TMSNumberOfRegisteredCarriersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNewAccountsRegisteredWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NewAccountsRegisteredWidget, "TMSNewAccountsRegisteredWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNewTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NewTripsWidget, "TMSNewTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfDeliveredTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfDeliveredTripsWidget, "TMSNumberOfDeliveredTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfOngoingTripsWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfOngoingTripsWidget, "TMSNumberOfOngoingTripsWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerTruckTypeUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TruckTypeUsageWidget, "TMSTruckTypeUsageWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerGoodTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.GoodTypesUsageWidget, "TMSGoodTypesUsageWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerRouteTypesUsageWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RouteTypesUsageWidget, "TMSRouteTypesUsageWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerMostRequestingShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.MostRequestingShippersWidget, "TMSMostRequestingShippersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerMostRequestedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.MostRequestedCarriersWidget, "TMSMostRequestedCarriersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerTopRatedShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TopRatedShippersWidget, "TMSTopRatedShippersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerTopRatedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.TopRatedCarriersWidget, "TMSTopRatedCarriersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerWorstRatedShippersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.WorstRatedShippersWidget, "TMSWorstRatedShippersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerWorstRatedCarriersWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.WorstRatedCarriersWidget, "TMSWorstRatedCarriersWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerUnPricedRequestsInMarketPlaceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.UnPricedRequestsInMarketPlaceWidget, "TMSUnPricedRequestsInMarketPlaceWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerRequestsPricingBeforeBidEndingWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RequestsPricingBeforeBidEndingWidget, "TMSRequestsPricingBeforeBidEndingWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerRequestsPriceAcceptanceWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.RequestsPriceAcceptanceWidget, "TMSRequestsPriceAcceptanceWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerInvoicesPaidBeforeDueDateWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.InvoicesPaidBeforeDueDateWidget, "TMSInvoicesPaidBeforeDueDateWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
            var tachyonDealerNumberOfRequestsPerAreaOrCityWidget = new WidgetDefinition(TACHYONDashboardCustomizationConsts.Widgets.TachyonDealer.NumberOfRequestsPerAreaOrCityWidget, "TMSNumberOfRequestsPerAreaOrCityWidget", side: MultiTenancySides.Tenant, permissions: tachyonDealerWidgetsDefaultPermission);
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
                    //carrierMostUsedPpWidget.Id,
                    carrierMostUsedVasWidget.Id,
                    carrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget.Id,
                    carrierMostWorkedWithShipperWidget.Id,
                    //carrierNextInvoiceFrequenctEndDateWidget.Id,
                    carrierDueDateInDaysWidget.Id,
                    carrierTrackingMapWidget.Id,
                    CarrierMostUsedPPWidget.Id
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
                    //shipperNextInvoiceFrequancyEndDateWidget.Id,
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
                    hostNumberOfRegisteredTrucksWidget.Id,
                    hostNumberOfRegisteredShippersWidget.Id,
                    hostNumberOfRegisteredCarriersWidget.Id,
                    hostNewAccountsRegisteredWidget.Id,
                    hostNewTripsWidget.Id,
                    hostNumberOfDeliveredTripsWidget.Id,
                    hostNumberOfOngoingTripsWidget.Id,
                    hostTruckTypeUsageWidget.Id,
                    hostGoodTypesUsageWidget.Id,
                    hostRouteTypesUsageWidget.Id,
                    hostMostRequestingShippersWidget.Id,
                    hostMostRequestedCarriersWidget.Id,
                    hostTopRatedShippersWidget.Id,
                    hostTopRatedCarriersWidget.Id,
                    hostWorstRatedShippersWidget.Id,
                    hostWorstRatedCarriersWidget.Id,
                    hostUnPricedRequestsInMarketPlaceWidget.Id,
                    hostRequestsPricingBeforeBidEndingWidget.Id,
                    hostRequestsPriceAcceptanceWidget.Id,
                    hostInvoicesPaidBeforeDueDateWidget.Id,
                    hostNumberOfRequestsPerAreaOrCityWidget.Id,
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