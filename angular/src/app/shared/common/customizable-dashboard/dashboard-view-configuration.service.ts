import { Injectable, OnInit } from '@angular/core';
import { WidgetViewDefinition, WidgetFilterViewDefinition } from './definitions';
import { DashboardCustomizationConst } from './DashboardCustomizationConsts';
import { WidgetGeneralStatsComponent } from './widgets/widget-general-stats/widget-general-stats.component';
import { WidgetDailySalesComponent } from './widgets/widget-daily-sales/widget-daily-sales.component';
import { WidgetProfitShareComponent } from './widgets/widget-profit-share/widget-profit-share.component';
import { WidgetMemberActivityComponent } from './widgets/widget-member-activity/widget-member-activity.component';
import { WidgetRegionalStatsComponent } from './widgets/widget-regional-stats/widget-regional-stats.component';
import { WidgetSalesSummaryComponent } from './widgets/widget-sales-summary/widget-sales-summary.component';
import { WidgetIncomeStatisticsComponent } from './widgets/widget-income-statistics/widget-income-statistics.component';
import { WidgetRecentTenantsComponent } from './widgets/widget-recent-tenants/widget-recent-tenants.component';
import { WidgetEditionStatisticsComponent } from './widgets/widget-edition-statistics/widget-edition-statistics.component';
import { WidgetSubscriptionExpiringTenantsComponent } from './widgets/widget-subscription-expiring-tenants/widget-subscription-expiring-tenants.component';
import { WidgetHostTopStatsComponent } from './widgets/widget-host-top-stats/widget-host-top-stats.component';
import { FilterDateRangePickerComponent } from './filters/filter-date-range-picker/filter-date-range-picker.component';
import { WidgetTopStatsComponent } from './widgets/widget-top-stats/widget-top-stats.component';
import { CompletedTripsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/completed-trips-widget/completed-trips-widget.component';
import { CompletedTripVsPodComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/completed-trip-vs-pod/completed-trip-vs-pod.component';
import { AcceptedVsRejecedRequestsComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/accepted-vs-rejeced-requests/accepted-vs-rejeced-requests.component';
import { InvoicesVsPaidInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/invoices-vs-paid-invoices/invoices-vs-paid-invoices.component';
import { MostWorkedWithCarriersComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-worked-with-carriers/most-worked-with-carriers.component';
import { RequestsInMarketPlaceComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/requests-in-market-place/requests-in-market-place.component';
import { NextInvoiceFrequancyDateComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/next-invoice-frequancy-date/next-invoice-frequancy-date.component';
import { InvoiceDueDateComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/invoice-due-date/invoice-due-date.component';
import { ShipperDueDateInDaysComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/shipper-due-date-in-days/shipper-due-date-in-days.component';
import { MostUsedOriginComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-used-origin/most-used-origin.component';
import { MostUsedDestinationsComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-used-destinations/most-used-destinations.component';
import { TrackingMapComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/tracking-map/tracking-map.component';
import { AcceptedVsRejectedPricingComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/accepted-vs-rejected-pricing/accepted-vs-rejected-pricing.component';
import { MostUsedVasesComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/most-used-vases/most-used-vases.component';
import { MostWorkedWithShippersComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/most-worked-with-shippers/most-worked-with-shippers.component';
import { MostUsedppComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/most-usedpp/most-usedpp.component';
import { TucksActivityComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/tucks-activity/tucks-activity.component';
import { DriversActivityComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/drivers-activity/drivers-activity.component';
import { NumberOfRegesterdTrucksComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-regesterd-trucks/number-of-regesterd-trucks.component';
import { NumberOfRegesterdShippersComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-regesterd-shippers/number-of-regesterd-shippers.component';
import { OnGoingTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/on-going-trips/on-going-trips.component';
import { DeleverdTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/deleverd-trips/deleverd-trips.component';
import { NumberOfRegesterdCarriersComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-regesterd-carriers/number-of-regesterd-carriers.component';
import { HostNewAccountsChartComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-new-accounts-chart/host-new-accounts-chart.component';
import { HostNewTripsChartComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-new-trips-chart/host-new-trips-chart.component';
import { HostTruckTypeUsageChartComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-truck-type-usage-chart/host-truck-type-usage-chart.component';
import { HostGoodTypesUsageChartComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-good-types-usage-chart/host-good-types-usage-chart.component';
import { HostRquestPricingMeterComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-rquest-pricing-meter/host-rquest-pricing-meter.component';
import { HostRquestAcceptanceMeterComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-rquest-acceptance-meter/host-rquest-acceptance-meter.component';
import { HostInvoicesMeterComponent } from '@app/shared/common/customizable-dashboard/widgets/host/host-invoices-meter/host-invoices-meter.component';
import { WorstRatedCarriersComponent } from './widgets/host/worst-rated-carriers/worst-rated-carriers.component';
import { UnpricedRequestsInMarketplaceComponent } from './widgets/host/unpriced-requests-in-marketplace/unpriced-requests-in-marketplace.component';
import { WorstRatedShippersComponent } from './widgets/host/worst-rated-shippers/worst-rated-shippers.component';
import { TopRatedCarriersComponent } from './widgets/host/top-rated-carriers/top-rated-carriers.component';
import { TopRatedShippersComponent } from './widgets/host/top-rated-shippers/top-rated-shippers.component';
import { TopThreeCarriersHaveRequestsComponent } from './widgets/host/top-three-carriers-have-requests/top-three-carriers-have-requests.component';
import { TopThreeShippersHaveRequestsComponent } from './widgets/host/top-three-shippers-have-requests/top-three-shippers-have-requests.component';
import { HostRouteTypeUsageChartComponent } from './widgets/host/host-route-type-usage-chart/host-route-type-usage-chart.component';
import { NumberOfRequestsForEachCityComponent } from './widgets/host/number-of-requests-for-each-city/number-of-requests-for-each-city.component';

@Injectable({
  providedIn: 'root',
})
export class DashboardViewConfigurationService {
  public WidgetViewDefinitions: WidgetViewDefinition[] = [];
  public widgetFilterDefinitions: WidgetFilterViewDefinition[] = [];

  constructor() {
    this.initializeConfiguration();
  }

  private initializeConfiguration() {
    let filterDateRangePicker = new WidgetFilterViewDefinition(
      DashboardCustomizationConst.filters.filterDateRangePicker,
      FilterDateRangePickerComponent
    );
    //add your filters here
    this.widgetFilterDefinitions.push(filterDateRangePicker);

    //Shipper
    //1
    let shippercompletedTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperNumberOfCompletedTripsWidget,
      CompletedTripsWidgetComponent
    );
    //2
    let acceptedVsRejectedRequestsWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperAcceptedVsRejectedRequestsWidget,
      AcceptedVsRejecedRequestsComponent
    );

    //3
    let shipperCompletedTripsVsPodWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperCompletedTripsVsPodWidget,
      CompletedTripVsPodComponent
    );
    //4
    let shipperInvoicesVsPaidInvoices = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperInvoicesVsPaidInvoicesWidget,
      InvoicesVsPaidInvoicesComponent
    );

    //5
    let mostWorkedWithCarriers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostWorkedWithCarriersWidget,
      MostWorkedWithCarriersComponent
    );

    //5
    let shippingRequestsInMarketPlace = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperRequestsInMarketplaceWidget,
      RequestsInMarketPlaceComponent
    );
    //6
    let nextInvoiceFrequancyDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperNextInvoiceFrequancyEndDateWidget,
      NextInvoiceFrequancyDateComponent,
      6,
      5
    );
    //7
    let invoiceDueDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperInvoiceDueDateInDaysWidget,
      InvoiceDueDateComponent,
      4,
      3
    );
    //8
    let dueDateInDays = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperDocumentDueDateInDaysWidget,
      ShipperDueDateInDaysComponent,
      4,
      3
    );

    //9
    let mostUsedOrigins = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostUsedOriginsWidget,
      MostUsedOriginComponent,
      4,
      6
    );

    //9
    let mostUsedDest = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostUsedDestinationsWidget,
      MostUsedDestinationsComponent,
      4,
      6
    );

    let trackingMapOfShipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperTrackingMapWidget,
      TrackingMapComponent,
      8,
      8
    );

    //carrier Widgets
    let carrierInvoicesVsPaid = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierInvoicesVsPaidInvoicesWidget,
      InvoicesVsPaidInvoicesComponent
    );

    let carrierAcceptedVsRejected = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierAcceptedVsRejectedPricingWidget,
      AcceptedVsRejectedPricingComponent
    );

    let CarrierCompletedTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget,
      CompletedTripsWidgetComponent
    );

    let mostUsedVases = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Carrier.CarrierMostUsedVasWidget, MostUsedVasesComponent);

    let mostWorkedWithShippers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierMostWorkedWithShipperWidget,
      MostWorkedWithShippersComponent
    );

    let mostUsedPP = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Carrier.CarrierMostUsedPpWidget, MostUsedppComponent);

    let CarrierNextInvoice = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierNextInvoiceFrequenctEndDateWidget,
      NextInvoiceFrequancyDateComponent
    );

    let CarrierDocumentsDueDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierDueDateInDaysWidget,
      ShipperDueDateInDaysComponent
    );

    let activeTrucks = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Carrier.CarrierTrucksActivityWidget, TucksActivityComponent);

    let activeDrivers = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Carrier.CarrierDriversActivityWidget, DriversActivityComponent);

    let trackingMapOfCarrier = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierTrackingMapWidget,
      TrackingMapComponent,
      8,
      5
    );
    //Host

    let NumberOfRegisteredTrucks = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredTrucksWidget,
      NumberOfRegesterdTrucksComponent,
      6,
      5
    );

    let NumberOfRegisteredShippers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredShippersWidget,
      NumberOfRegesterdShippersComponent,
      6,
      5
    );

    let onGoingTrips = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.NumberOfOngoingTripsWidget, OnGoingTripsComponent, 8, 4);

    let TopThreeShippersHaveRequests = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.MostRequestingShippersWidget,
      TopThreeShippersHaveRequestsComponent,
      8,
      10
    );

    let TopThreeCarriersHaveRequests = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.MostRequestedCarriersWidget,
      TopThreeCarriersHaveRequestsComponent,
      8,
      10
    );

    let topRatedShippersWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.TopRatedShippersWidget,
      TopRatedShippersComponent,
      8,
      15
    );

    let topRatedCarriersWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.TopRatedCarriersWidget,
      TopRatedCarriersComponent,
      8,
      15
    );

    let worstRatedShippersWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.WorstRatedShippersWidget,
      WorstRatedShippersComponent,
      8,
      15
    );

    let worstRatedCarriersWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.WorstRatedCarriersWidget,
      WorstRatedCarriersComponent,
      8,
      15
    );

    let unPricedShippingRequestsWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.UnPricedRequestsInMarketPlaceWidget,
      UnpricedRequestsInMarketplaceComponent,
      10,
      15
    );

    let numberOfDeliveredTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfDeliveredTripsWidget,
      DeleverdTripsComponent,
      6,
      5
    );

    let numberOfRegisteredCarriers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredCarriersWidget,
      NumberOfRegesterdCarriersComponent,
      6,
      5
    );

    let hostNewAccounts = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NewAccountsRegisteredWidget,
      HostNewAccountsChartComponent,
      10,
      15
    );
    let hostNewTrips = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.NewTripsWidget, HostNewTripsChartComponent, 18, 15);

    let hostTruckTypesChart = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.TruckTypeUsageWidget,
      HostTruckTypeUsageChartComponent,
      10,
      15
    );

    let goodTypesUsage = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.GoodTypesUsageWidget,
      HostGoodTypesUsageChartComponent,
      10,
      15
    );

    let routeTypeUsage = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.RouteTypesUsageWidget,
      HostRouteTypeUsageChartComponent,
      10,
      15
    );

    let hostRequestPricedMeter = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.RequestsPricingBeforeBidEndingWidget,
      HostRquestPricingMeterComponent,
      10,
      15
    );
    let hostRequestAcceptanceMeter = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.RequestsPriceAcceptanceWidget,
      HostRquestAcceptanceMeterComponent,
      10,
      15
    );

    let hostInvoicePaidMeter = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.InvoicesPaidBeforeDueDateWidget,
      HostInvoicesMeterComponent,
      10,
      15
    );

    let hostRequestsPerAreaOrCity = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRequestsPerAreaOrCityWidget,
      NumberOfRequestsForEachCityComponent,
      18,
      10
    );

    //TMS
    let NumberOfRegisteredTrucks_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNumberOfRegisteredTrucksWidget,
      NumberOfRegesterdTrucksComponent,
      6,
      5
    );

    let NumberOfRegisteredShippers_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNumberOfRegisteredShippersWidget,
      NumberOfRegesterdShippersComponent,
      6,
      5
    );

    let onGoingTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNumberOfOngoingTripsWidget,
      OnGoingTripsComponent,
      6,
      5
    );

    let numberOfDeliveredTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNumberOfDeliveredTripsWidget,
      DeleverdTripsComponent,
      6,
      5
    );

    let numberOfRegisteredCarriers_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSMostRequestedCarriersWidget,
      NumberOfRegesterdCarriersComponent,
      6,
      5
    );

    let NewAccounts_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNewAccountsRegisteredWidget,
      HostNewAccountsChartComponent,
      10,
      15
    );
    let NewTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNewTripsWidget,
      HostNewTripsChartComponent,
      10,
      15
    );

    let TruckTypesChart_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSTruckTypeUsageWidget,
      HostTruckTypeUsageChartComponent,
      10,
      15
    );

    let goodTypesUsage_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSGoodTypesUsageWidget,
      HostGoodTypesUsageChartComponent,
      10,
      15
    );

    let routeTypeUsage_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSRouteTypesUsageWidget,
      HostRouteTypeUsageChartComponent,
      10,
      15
    );

    let RequestPricedMeter_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSRequestsPricingBeforeBidEndingWidget,
      HostRquestPricingMeterComponent,
      10,
      15
    );
    let RequestAcceptanceMeter_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSRequestsPriceAcceptanceWidget,
      HostRquestAcceptanceMeterComponent,
      8,
      8
    );

    let InvoicePaidMeter_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSInvoicesPaidBeforeDueDateWidget,
      HostInvoicesMeterComponent,
      8,
      8
    );

    let RequestsPerAreaOrCity_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNumberOfRequestsPerAreaOrCityWidget,
      NumberOfRequestsForEachCityComponent,
      18,
      20
    );

    //shipperPush
    this.WidgetViewDefinitions.push(shippercompletedTrips);
    this.WidgetViewDefinitions.push(acceptedVsRejectedRequestsWidget);
    this.WidgetViewDefinitions.push(shipperCompletedTripsVsPodWidget);
    this.WidgetViewDefinitions.push(shipperInvoicesVsPaidInvoices);
    this.WidgetViewDefinitions.push(mostWorkedWithCarriers);
    this.WidgetViewDefinitions.push(shippingRequestsInMarketPlace);
    this.WidgetViewDefinitions.push(nextInvoiceFrequancyDate);
    this.WidgetViewDefinitions.push(invoiceDueDate);
    this.WidgetViewDefinitions.push(dueDateInDays);
    this.WidgetViewDefinitions.push(mostUsedOrigins);
    this.WidgetViewDefinitions.push(mostUsedDest);
    this.WidgetViewDefinitions.push(trackingMapOfShipper);

    //Carrier
    this.WidgetViewDefinitions.push(carrierInvoicesVsPaid);
    this.WidgetViewDefinitions.push(carrierAcceptedVsRejected);
    this.WidgetViewDefinitions.push(CarrierCompletedTrips);
    this.WidgetViewDefinitions.push(mostWorkedWithShippers);
    this.WidgetViewDefinitions.push(mostUsedPP);
    this.WidgetViewDefinitions.push(CarrierNextInvoice);
    this.WidgetViewDefinitions.push(CarrierDocumentsDueDate);
    this.WidgetViewDefinitions.push(activeTrucks);
    this.WidgetViewDefinitions.push(activeDrivers);
    this.WidgetViewDefinitions.push(mostUsedVases);
    this.WidgetViewDefinitions.push(trackingMapOfCarrier);

    //TMS
    this.widgetFilterDefinitions.push(NumberOfRegisteredTrucks);
    this.WidgetViewDefinitions.push(NumberOfRegisteredTrucks_TMS);
    this.WidgetViewDefinitions.push(NumberOfRegisteredShippers_TMS);
    this.WidgetViewDefinitions.push(onGoingTrips_TMS);
    this.WidgetViewDefinitions.push(numberOfDeliveredTrips_TMS);
    this.WidgetViewDefinitions.push(numberOfRegisteredCarriers_TMS);
    this.WidgetViewDefinitions.push(NewAccounts_TMS);
    this.WidgetViewDefinitions.push(NewTrips_TMS);
    this.WidgetViewDefinitions.push(TruckTypesChart_TMS);
    this.WidgetViewDefinitions.push(goodTypesUsage_TMS);
    this.WidgetViewDefinitions.push(routeTypeUsage_TMS);
    this.WidgetViewDefinitions.push(RequestPricedMeter_TMS);
    this.WidgetViewDefinitions.push(RequestAcceptanceMeter_TMS);
    this.WidgetViewDefinitions.push(InvoicePaidMeter_TMS);
    this.WidgetViewDefinitions.push(RequestsPerAreaOrCity_TMS);
    //Host
    //this.widgetFilterDefinitions.push(NumberOfRegisteredTrucks);

    this.WidgetViewDefinitions.push(NumberOfRegisteredTrucks);
    this.WidgetViewDefinitions.push(NumberOfRegisteredShippers);
    this.WidgetViewDefinitions.push(onGoingTrips);
    this.WidgetViewDefinitions.push(numberOfDeliveredTrips);
    this.WidgetViewDefinitions.push(numberOfRegisteredCarriers);

    this.WidgetViewDefinitions.push(hostNewAccounts);
    this.WidgetViewDefinitions.push(hostNewTrips);
    this.WidgetViewDefinitions.push(hostTruckTypesChart);
    this.WidgetViewDefinitions.push(goodTypesUsage);
    this.WidgetViewDefinitions.push(routeTypeUsage);

    this.WidgetViewDefinitions.push(hostRequestPricedMeter);
    this.WidgetViewDefinitions.push(hostRequestAcceptanceMeter);
    this.WidgetViewDefinitions.push(hostInvoicePaidMeter);
    this.WidgetViewDefinitions.push(TopThreeShippersHaveRequests);
    this.WidgetViewDefinitions.push(TopThreeCarriersHaveRequests);
    this.WidgetViewDefinitions.push(topRatedShippersWidget);
    this.WidgetViewDefinitions.push(topRatedCarriersWidget);
    this.WidgetViewDefinitions.push(worstRatedShippersWidget);
    this.WidgetViewDefinitions.push(worstRatedCarriersWidget);
    this.WidgetViewDefinitions.push(unPricedShippingRequestsWidget);
    this.WidgetViewDefinitions.push(hostRequestsPerAreaOrCity);
    //console.log('WidgetViewDefinitions', this.WidgetViewDefinitions);
    // this.widgetFilterDefinitions.push();
  }
}
