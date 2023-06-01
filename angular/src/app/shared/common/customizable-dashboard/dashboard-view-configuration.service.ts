import { Injectable } from '@angular/core';
import { WidgetFilterViewDefinition, WidgetViewDefinition } from './definitions';
import { DashboardCustomizationConst } from './DashboardCustomizationConsts';
import { FilterDateRangePickerComponent } from './filters/filter-date-range-picker/filter-date-range-picker.component';
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
import { CarrierComplitedTripsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/carrier-complited-trips-widget/carrier-complited-trips-widget.component';
import { CarrierAcceptedVsRejectdRequestsComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/carrier-accepted-vs-rejectd-requests/carrier-accepted-vs-rejectd-requests.component';
import { CarrierInvoicesDetailsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/carrier-invoices-details-widget/carrier-invoices-details-widget.component';
import { CountersWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/counters-widget/counters-widget.component';
import { UpcomingTripsWidgetsComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/upcoming-trips-widgets/upcoming-trips-widgets.component';
import { NeedsActionWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/needs-action-widget/needs-action-widget.component';
import { NewOffersWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/new-offers-widget/new-offers-widget.component';
import { DriverTucksActivityComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/driver-tucks-activity/driver-tucks-activity.component';
import { AcceptedVsRejectedPricingComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/accepted-vs-rejected-pricing/accepted-vs-rejected-pricing.component';
import { NewDirectRequestsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/new-direct-requests-widget/new-direct-requests-widget.component';
import { NewActorsThisMonthComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/new-actors-this-month/new-actors-this-month.component';
import { NumberOfActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/number-of-actors/number-of-actors.component';
import { MostActiveActorShipperComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-active-actor-shipper/most-active-actor-shipper.component';
import { MostActiveActorCarrierComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-active-actor-carrier/most-active-actor-carrier.component';
import { NumberOfActiveActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/number-of-active-actors/number-of-active-actors.component';
import { MostUsedOriginsActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-used-origins-actors/most-used-origins-actors.component';
import { MostUsedDestinationsActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-used-destinations-actors/most-used-destinations-actors.component';
import { ActorUpcomingTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-upcoming-trips/actor-upcoming-trips.component';
import { ActorNeedsActionsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-needs-actions/actor-needs-actions.component';
import { ActorPendingPriceOffersComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-pending-price-offers/actor-pending-price-offers.component';
import { ActorMostTruckTypeUsedComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-most-truck-type-used/actor-most-truck-type-used.component';
import { ActorNextDocDueDateComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-next-doc-due-date/actor-next-doc-due-date.component';
import { ActorNextInvoiceDueDateComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-next-invoice-due-date/actor-next-invoice-due-date.component';
import { ActorNewInvoiceVsPaidInvoiceComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-new-invoice-vs-paid-invoice/actor-new-invoice-vs-paid-invoice.component';
import { ActorPaidInvoiceVsClaimedInvoiceComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/actor-paid-invoice-vs-claimed-invoice/actor-paid-invoice-vs-claimed-invoice.component';
import { NumberOfRegisteredCompaniesComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-registered-companies/number-of-registered-companies.component';
import { NumberOfDriversAndTrucksComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-drivers-and-trucks/number-of-drivers-and-trucks.component';
import { TopRatedShippersAndCarriersComponent } from '@app/shared/common/customizable-dashboard/widgets/host/top-rated-shippers-and-carriers/top-rated-shippers-and-carriers.component';
import { NormalRequestsVSDedicatedRequestsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/normal-requests-vs-dedicated-requests/normal-requests-vs-dedicated-requests.component';
import { NumberOfTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-trips/number-of-trips.component';
import { NumberOfSaasTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-saas-trips/number-of-saas-trips.component';
import { NumberOfTruckAggregationTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-truck-aggregation-trips/number-of-truck-aggregation-trips.component';
import { NumberOfTruckAggregationTripsVsSaasTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-truck-aggregation-trips-vs-saas-trips/number-of-truck-aggregation-trips-vs-saas-trips.component';
import { TopWorstRatedPerTripComponent } from '@app/shared/common/customizable-dashboard/widgets/host/top-worst-rated-per-trip/top-worst-rated-per-trip.component';
import { PaidInvoicesBeforeDueDateComponent } from '@app/shared/common/customizable-dashboard/widgets/host/paid-invoices-before-due-date/paid-invoices-before-due-date.component';
import { CostVsSellingVsProfitOfSaasTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/cost-vs-selling-vs-profit-of-saas-trips/cost-vs-selling-vs-profit-of-saas-trips.component';
import { OverallTotalAmountPerAllTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/overall-total-amount-per-all-trips/overall-total-amount-per-all-trips.component';
import { TruckAggregationInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/host/truck-aggregation-invoices/truck-aggregation-invoices.component';
import { SaasInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/host/saas-invoices/saas-invoices.component';
import { GoodTypesUsageComponent } from '@app/shared/common/customizable-dashboard/widgets/host/good-types-usage/good-types-usage.component';
import { CostVsSellingVsProfitOfTruckAggregationTripsComponent } from '@app/shared/common/customizable-dashboard/widgets/host/cost-vs-selling-vs-profit-of-truck-aggregation-trips/cost-vs-selling-vs-profit-of-truck-aggregation-trips.component';
import { NewRegisteredCompaniesComponent } from '@app/shared/common/customizable-dashboard/widgets/host/new-registered-companies/new-registered-companies.component';

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
      AcceptedVsRejecedRequestsComponent,
      18,
      10
    );

    //3
    let shipperCompletedTripsVsPodWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperCompletedTripsVsPodWidget,
      CompletedTripVsPodComponent,
      18,
      10
    );
    //4
    let shipperInvoicesVsPaidInvoices = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperInvoicesVsPaidInvoicesWidget,
      InvoicesVsPaidInvoicesComponent,
      14,
      10
    );

    //9
    let mostUsedOrigins = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostUsedOriginsWidget,
      MostUsedOriginComponent,
      6,
      10
    );

    //9
    let mostUsedDest = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperMostUsedDestinationsWidget,
      MostUsedDestinationsComponent,
      6,
      10
    );

    let trackingMapOfShipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperTrackingMapWidget,
      TrackingMapComponent,
      8,
      8
    );

    let countersWidget_Shipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.ShipperCountersWidget,
      CountersWidgetComponent,
      6,
      10
    );
    let upcomingTripsWidget_Shipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.UpcomingTripsWidget,
      UpcomingTripsWidgetsComponent,
      6,
      10
    );
    let needsActionWidget_Shipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.NeedsActionWidget,
      NeedsActionWidgetComponent,
      6,
      10
    );
    let newOffersWidget_Shipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Shipper.NewOffersWidget,
      NewOffersWidgetComponent,
      10,
      10
    );

    //carrier Widgets
    let carrierInvoicesVsPaid = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierInvoicesVsPaidInvoicesWidget,
      CarrierInvoicesDetailsWidgetComponent,
      14,
      10
    );

    let carrierAcceptedVsRejected = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierAcceptedVsRejectedPricingWidget,
      AcceptedVsRejectedPricingComponent,
      18,
      10
    );

    let CarrierCompletedTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierNumberOfCompletedTripsTotalMonthlyIncreaseWidget,
      CarrierComplitedTripsWidgetComponent
    );

    let mostUsedVases = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Carrier.CarrierMostUsedVasWidget, MostUsedVasesComponent, 6, 10);

    let mostUsedPP = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Carrier.CarrierMostUsedPpWidget, MostUsedppComponent, 6, 10);

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
    let activeDriversAndTrucks = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierActiveDriversAndTrucksWidget,
      DriverTucksActivityComponent,
      18,
      10
    );
    let carrierUpcomingTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierUpcomingTripsWidget,
      UpcomingTripsWidgetsComponent,
      6,
      10
    );
    let carrierNeedsActionWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.NeedsActionWidget,
      NeedsActionWidgetComponent,
      6,
      7
    );
    let carrierNewDirectRequestsWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.NewDirectRequestsWidget,
      NewDirectRequestsWidgetComponent,
      10,
      10
    );

    let trackingMapOfCarrier = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierTrackingMapWidget,
      TrackingMapComponent,
      8,
      5
    );

    let countersWidget_Carrier = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Carrier.CarrierCountersWidget,
      CountersWidgetComponent,
      6,
      10
    );

    //Host

    let NumberOfRegisteredTrucks = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredTrucksWidget,
      NumberOfRegesterdTrucksComponent,
      6,
      5
    );

    let NumberOfDriversAndTrucks = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfDriversAndTrucksWidget,
      NumberOfDriversAndTrucksComponent,
      9,
      10
    );

    let NumberOfTrips = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.NumberOfTripsWidget, NumberOfTripsComponent, 9, 10);

    let NormalRequestsVSDedicatedRequests = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NormalRequestsVSDedicatedRequestsWidget,
      NormalRequestsVSDedicatedRequestsComponent,
      12,
      10
    );

    let NumberOfTruckAggregationTripsVsSaasTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfTruckAggregationTripsVsSaasTrips,
      NumberOfTruckAggregationTripsVsSaasTripsComponent,
      12,
      10
    );

    let NumberOfRegisteredShippers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredShippersWidget,
      NumberOfRegesterdShippersComponent,
      6,
      5
    );

    let onGoingTrips = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.NumberOfOngoingTripsWidget, OnGoingTripsComponent, 3, 5);

    let NumberOfSaasTrips = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.NumberOfSaasTrips, NumberOfSaasTripsComponent, 3, 5);

    let NumberOfTruckAggregationTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfTruckAggregationTrips,
      NumberOfTruckAggregationTripsComponent,
      3,
      5
    );

    let OverallTotalAmountPerAllTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.OverallTotalAmountPerAllTrips,
      OverallTotalAmountPerAllTripsComponent,
      4,
      12
    );

    let TruckAggregationInvoices = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.TruckAggregationInvoices,
      TruckAggregationInvoicesComponent,
      6,
      7
    );

    let SaasInvoices = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.SaasInvoices, SaasInvoicesComponent, 6, 7);

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

    let topRatedShippersAndCarriersWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.TopRatedShippersAndCarriersWidget,
      TopRatedShippersAndCarriersComponent,
      6,
      13
    );

    let TopWorstRatedPerTripWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.TopWorstRatedPerTrip,
      TopWorstRatedPerTripComponent,
      6,
      15
    );

    let PaidInvoicesBeforeDueDateWidget = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.PaidInvoicesBeforeDueDate,
      PaidInvoicesBeforeDueDateComponent,
      6,
      9
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
      3,
      5
    );

    let numberOfRegisteredCarriers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredCarriersWidget,
      NumberOfRegesterdCarriersComponent,
      6,
      5
    );

    let numberOfRegisteredCompanies = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NumberOfRegisteredCompaniesWidget,
      NumberOfRegisteredCompaniesComponent,
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

    let goodTypesUsage = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.GoodTypesUsageWidget, GoodTypesUsageComponent, 10, 15);

    let routeTypeUsage = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.RouteTypesUsageWidget,
      HostRouteTypeUsageChartComponent,
      6,
      5
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
      12,
      12
    );

    let HostMostTruckTypeUsed = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.MostTruckTypeUsedWidget,
      ActorMostTruckTypeUsedComponent,
      12,
      12
    );

    let GoodTypesUsage = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.GoodTypesUsage, GoodTypesUsageComponent, 12, 12);
    let CostVsSellingVsProfitOfSaasTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.CostVsSellingVsProfitOfSaasTripsWidget,
      CostVsSellingVsProfitOfSaasTripsComponent,
      18,
      12
    );
    let NewRegisteredCompanies = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.NewRegisteredCompanies,
      NewRegisteredCompaniesComponent,
      9,
      10
    );

    let CostVsSellingVsProfitOfTruckAggregationTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.CostVsSellingVsProfitOfTruckAggregationTripsWidget,
      CostVsSellingVsProfitOfTruckAggregationTripsComponent,
      14,
      12
    );

    let InvoicesVsPaidInvoices_Host = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.InvoicesVsPaidInvoices,
      InvoicesVsPaidInvoicesComponent,
      9,
      10
    );
    let carrierInvoicesVsPaid_Host = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.host.ClaimedInvoicesVsPaidInvoices,
      CarrierInvoicesDetailsWidgetComponent,
      9,
      10
    );
    let HostUpcomingTrips = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.UpcomingTrips, UpcomingTripsWidgetsComponent, 6, 15);
    let HostNeedsActions = new WidgetViewDefinition(DashboardCustomizationConst.widgets.host.NeedsActions, NeedsActionWidgetComponent, 6, 15);

    //TMS
    let NumberOfRegisteredTrucks_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfRegisteredTrucksWidget,
      NumberOfRegesterdTrucksComponent,
      6,
      5
    );

    let NumberOfRegisteredShippers_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfRegisteredShippersWidget,
      NumberOfRegesterdShippersComponent,
      6,
      5
    );

    let onGoingTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfOngoingTripsWidget,
      OnGoingTripsComponent,
      3,
      5
    );

    let NumberOfSaasTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfSaasTrips,
      NumberOfSaasTripsComponent,
      3,
      5
    );

    let NumberOfTruckAggregationTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfTruckAggregationTrips,
      NumberOfTruckAggregationTripsComponent,
      3,
      5
    );

    let OverallTotalAmountPerAllTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.OverallTotalAmountPerAllTrips,
      OverallTotalAmountPerAllTripsComponent,
      4,
      12
    );

    let TruckAggregationInvoices_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TruckAggregationInvoices,
      TruckAggregationInvoicesComponent,
      6,
      7
    );

    let SaasInvoices_TMS = new WidgetViewDefinition(DashboardCustomizationConst.widgets.TachyonDealer.SaasInvoices, SaasInvoicesComponent, 6, 7);

    let numberOfDeliveredTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfDeliveredTripsWidget,
      DeleverdTripsComponent,
      3,
      5
    );

    let numberOfRegisteredCarriers_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.MostRequestedCarriersWidget,
      NumberOfRegesterdCarriersComponent,
      6,
      5
    );

    let numberOfRegisteredCompanies_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfRegisteredCompaniesWidget,
      NumberOfRegisteredCompaniesComponent,
      6,
      5
    );

    let NumberOfDriversAndTrucks_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfDriversAndTrucksWidget,
      NumberOfDriversAndTrucksComponent,
      9,
      10
    );

    let NumberOfTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfTripsWidget,
      NumberOfTripsComponent,
      9,
      10
    );

    let NormalRequestsVSDedicatedRequests_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NormalRequestsVSDedicatedRequestsWidget,
      NormalRequestsVSDedicatedRequestsComponent,
      12,
      10
    );

    let NumberOfTruckAggregationTripsVsSaasTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NumberOfTruckAggregationTripsVsSaasTrips,
      NumberOfTruckAggregationTripsVsSaasTripsComponent,
      12,
      10
    );

    let NewAccounts_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NewAccountsRegisteredWidget,
      HostNewAccountsChartComponent,
      10,
      15
    );
    let NewTrips_TMS = new WidgetViewDefinition(DashboardCustomizationConst.widgets.TachyonDealer.NewTripsWidget, HostNewTripsChartComponent, 10, 15);

    let TruckTypesChart_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TruckTypeUsageWidget,
      HostTruckTypeUsageChartComponent,
      10,
      15
    );

    let goodTypesUsage_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.GoodTypesUsageWidget,
      GoodTypesUsageComponent,
      10,
      15
    );

    let routeTypeUsage_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.RouteTypesUsageWidget,
      HostRouteTypeUsageChartComponent,
      6,
      5
    );

    let RequestPricedMeter_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSRequestsPricingBeforeBidEndingWidget,
      HostRquestPricingMeterComponent,
      10,
      15
    );
    let RequestAcceptanceMeter_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.RequestsPriceAcceptanceWidget,
      HostRquestAcceptanceMeterComponent,
      8,
      8
    );

    let InvoicePaidMeter_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.InvoicesPaidBeforeDueDateWidget,
      HostInvoicesMeterComponent,
      8,
      8
    );

    let RequestsPerAreaOrCity_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSNumberOfRequestsPerAreaOrCityWidget,
      NumberOfRequestsForEachCityComponent,
      12,
      12
    );

    let HostMostTruckTypeUsed_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.MostTruckTypeUsedWidget,
      ActorMostTruckTypeUsedComponent,
      12,
      12
    );

    let GoodTypesUsage_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.GoodTypesUsage,
      GoodTypesUsageComponent,
      12,
      12
    );

    let TopRatedShippersAndCarriersWidget_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TMSTopRatedShippersAndCarriersWidget,
      TopRatedShippersAndCarriersComponent,
      6,
      13
    );

    let TopWorstRatedPerTripWidget_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.TopWorstRatedPerTrip,
      TopWorstRatedPerTripComponent,
      6,
      15
    );

    let PaidInvoicesBeforeDueDateWidget_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.PaidInvoicesBeforeDueDate,
      PaidInvoicesBeforeDueDateComponent,
      6,
      9
    );

    let CostVsSellingVsProfitOfSaasTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.CostVsSellingVsProfitOfSaasTripsWidget,
      CostVsSellingVsProfitOfSaasTripsComponent,
      18,
      12
    );

    let NewRegisteredCompanies_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NewRegisteredCompanies,
      NewRegisteredCompaniesComponent,
      9,
      10
    );
    let CostVsSellingVsProfitOfTruckAggregationTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.CostVsSellingVsProfitOfTruckAggregationTripsWidget,
      CostVsSellingVsProfitOfTruckAggregationTripsComponent,
      14,
      12
    );

    let InvoicesVsPaidInvoices_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.InvoicesVsPaidInvoices,
      InvoicesVsPaidInvoicesComponent,
      9,
      10
    );
    let carrierInvoicesVsPaid_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.ClaimedInvoicesVsPaidInvoices,
      CarrierInvoicesDetailsWidgetComponent,
      9,
      10
    );

    let UpcomingTrips_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.UpcomingTrips,
      UpcomingTripsWidgetsComponent,
      6,
      15
    );
    let NeedsActions_TMS = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.TachyonDealer.NeedsActions,
      NeedsActionWidgetComponent,
      6,
      15
    );

    // broker dashboard
    let NewActorsThisMonth_Broker = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.NewActorsThisMonthWidget,
      NewActorsThisMonthComponent,
      12,
      5
    );
    let NumberOfActors_Broker = new WidgetViewDefinition(DashboardCustomizationConst.widgets.Broker.NumberOfActors, NumberOfActorsComponent, 12, 5);
    let MostActiveActorShipper = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.MostActiveActorShipper,
      MostActiveActorShipperComponent,
      12,
      10
    );
    let MostActiveActorCarrier = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.MostActiveActorCarrier,
      MostActiveActorCarrierComponent,
      12,
      10
    );
    let NumberOfActiveActors = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.NumberOfActiveActors,
      NumberOfActiveActorsComponent,
      8,
      10
    );
    let ActorsMostUsedOrigins = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorsMostUsedOrigins,
      MostUsedOriginsActorsComponent,
      8,
      12
    );

    let ActorsMostUsedDestinations = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorsMostUsedDestinations,
      MostUsedDestinationsActorsComponent,
      6,
      10
    );
    let ActorsUpcomingTrips = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorsUpcomingTrips,
      ActorUpcomingTripsComponent,
      6,
      8
    );
    let ActorsNeedsActions = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorsNeedsActions,
      ActorNeedsActionsComponent,
      6,
      7
    );
    let PendingPriceOffers = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.PendingPriceOffers,
      ActorPendingPriceOffersComponent,
      8,
      10
    );
    let MostTruckTypeUsed = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.MostTruckTypeUsed,
      ActorMostTruckTypeUsedComponent,
      16,
      12
    );
    let ActorNextDocDueDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorNextDocDueDate,
      ActorNextDocDueDateComponent,
      6,
      10
    );
    let ActorNextInvoiceDueDate = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorNextInvoiceDueDate,
      ActorNextInvoiceDueDateComponent,
      8,
      10
    );
    let NewInvoicesVsPaidInvoices = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.NewInvoicesVsPaidInvoices,
      ActorNewInvoiceVsPaidInvoiceComponent,
      15,
      10
    );
    let ActorsPaidInvoiceVsClaimedInvoice = new WidgetViewDefinition(
      DashboardCustomizationConst.widgets.Broker.ActorsPaidInvoiceVsClaimedInvoice,
      ActorPaidInvoiceVsClaimedInvoiceComponent,
      15,
      10
    );

    //shipperPush
    this.WidgetViewDefinitions.push(shippercompletedTrips);
    this.WidgetViewDefinitions.push(acceptedVsRejectedRequestsWidget);
    this.WidgetViewDefinitions.push(shipperCompletedTripsVsPodWidget);
    this.WidgetViewDefinitions.push(shipperInvoicesVsPaidInvoices);
    this.WidgetViewDefinitions.push(mostUsedOrigins);
    this.WidgetViewDefinitions.push(mostUsedDest);
    this.WidgetViewDefinitions.push(trackingMapOfShipper);
    this.WidgetViewDefinitions.push(countersWidget_Shipper);
    this.WidgetViewDefinitions.push(upcomingTripsWidget_Shipper);
    this.WidgetViewDefinitions.push(needsActionWidget_Shipper);
    this.WidgetViewDefinitions.push(newOffersWidget_Shipper);

    //Carrier
    this.WidgetViewDefinitions.push(carrierInvoicesVsPaid);
    this.WidgetViewDefinitions.push(carrierAcceptedVsRejected);
    this.WidgetViewDefinitions.push(CarrierCompletedTrips);
    this.WidgetViewDefinitions.push(mostUsedPP);
    this.WidgetViewDefinitions.push(CarrierNextInvoice);
    this.WidgetViewDefinitions.push(CarrierDocumentsDueDate);
    this.WidgetViewDefinitions.push(activeTrucks);
    this.WidgetViewDefinitions.push(activeDrivers);
    this.WidgetViewDefinitions.push(mostUsedVases);
    this.WidgetViewDefinitions.push(trackingMapOfCarrier);
    this.WidgetViewDefinitions.push(countersWidget_Carrier);
    this.WidgetViewDefinitions.push(activeDriversAndTrucks);
    this.WidgetViewDefinitions.push(carrierUpcomingTrips);
    this.WidgetViewDefinitions.push(carrierNeedsActionWidget);
    this.WidgetViewDefinitions.push(carrierNewDirectRequestsWidget);

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
    this.WidgetViewDefinitions.push(numberOfRegisteredCompanies_TMS);
    this.WidgetViewDefinitions.push(NumberOfDriversAndTrucks_TMS);
    this.WidgetViewDefinitions.push(TopRatedShippersAndCarriersWidget_TMS);
    this.WidgetViewDefinitions.push(NormalRequestsVSDedicatedRequests_TMS);
    this.WidgetViewDefinitions.push(HostMostTruckTypeUsed_TMS);
    this.WidgetViewDefinitions.push(NumberOfTrips_TMS);
    this.WidgetViewDefinitions.push(NumberOfSaasTrips_TMS);
    this.WidgetViewDefinitions.push(NumberOfTruckAggregationTrips_TMS);
    this.WidgetViewDefinitions.push(NumberOfTruckAggregationTripsVsSaasTrips_TMS);
    this.WidgetViewDefinitions.push(TopWorstRatedPerTripWidget_TMS);
    this.WidgetViewDefinitions.push(PaidInvoicesBeforeDueDateWidget_TMS);
    this.WidgetViewDefinitions.push(CostVsSellingVsProfitOfSaasTrips_TMS);
    this.WidgetViewDefinitions.push(OverallTotalAmountPerAllTrips_TMS);
    this.WidgetViewDefinitions.push(TruckAggregationInvoices_TMS);
    this.WidgetViewDefinitions.push(SaasInvoices_TMS);
    this.WidgetViewDefinitions.push(GoodTypesUsage_TMS);
    this.WidgetViewDefinitions.push(UpcomingTrips_TMS);
    this.WidgetViewDefinitions.push(NeedsActions_TMS);
    this.WidgetViewDefinitions.push(CostVsSellingVsProfitOfTruckAggregationTrips_TMS);
    this.WidgetViewDefinitions.push(NewRegisteredCompanies_TMS);
    this.WidgetViewDefinitions.push(InvoicesVsPaidInvoices_TMS);
    this.WidgetViewDefinitions.push(carrierInvoicesVsPaid_TMS);
    //Host
    //this.widgetFilterDefinitions.push(NumberOfRegisteredTrucks);

    this.WidgetViewDefinitions.push(NumberOfDriversAndTrucks);
    this.WidgetViewDefinitions.push(NumberOfTrips);
    this.WidgetViewDefinitions.push(NormalRequestsVSDedicatedRequests);
    this.WidgetViewDefinitions.push(NumberOfRegisteredTrucks);
    this.WidgetViewDefinitions.push(numberOfRegisteredCompanies);
    this.WidgetViewDefinitions.push(NumberOfRegisteredShippers);
    this.WidgetViewDefinitions.push(onGoingTrips);
    this.WidgetViewDefinitions.push(numberOfDeliveredTrips);
    this.WidgetViewDefinitions.push(numberOfRegisteredCarriers);
    this.WidgetViewDefinitions.push(NumberOfSaasTrips);
    this.WidgetViewDefinitions.push(NumberOfTruckAggregationTrips);
    this.WidgetViewDefinitions.push(NumberOfTruckAggregationTripsVsSaasTrips);

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
    this.WidgetViewDefinitions.push(topRatedShippersAndCarriersWidget);
    this.WidgetViewDefinitions.push(TopWorstRatedPerTripWidget);
    this.WidgetViewDefinitions.push(HostMostTruckTypeUsed);
    this.WidgetViewDefinitions.push(PaidInvoicesBeforeDueDateWidget);
    this.WidgetViewDefinitions.push(CostVsSellingVsProfitOfSaasTrips);
    this.WidgetViewDefinitions.push(OverallTotalAmountPerAllTrips);
    this.WidgetViewDefinitions.push(TruckAggregationInvoices);
    this.WidgetViewDefinitions.push(SaasInvoices);
    this.WidgetViewDefinitions.push(GoodTypesUsage);
    this.WidgetViewDefinitions.push(HostUpcomingTrips);
    this.WidgetViewDefinitions.push(HostNeedsActions);
    this.WidgetViewDefinitions.push(CostVsSellingVsProfitOfTruckAggregationTrips);
    this.WidgetViewDefinitions.push(NewRegisteredCompanies);
    this.WidgetViewDefinitions.push(InvoicesVsPaidInvoices_Host);
    this.WidgetViewDefinitions.push(carrierInvoicesVsPaid_Host);

    // broker
    this.WidgetViewDefinitions.push(NewActorsThisMonth_Broker);
    this.WidgetViewDefinitions.push(NumberOfActors_Broker);
    this.WidgetViewDefinitions.push(MostActiveActorShipper);
    this.WidgetViewDefinitions.push(MostActiveActorCarrier);
    this.WidgetViewDefinitions.push(NumberOfActiveActors);
    this.WidgetViewDefinitions.push(ActorsMostUsedOrigins);
    this.WidgetViewDefinitions.push(ActorsMostUsedDestinations);
    this.WidgetViewDefinitions.push(ActorsUpcomingTrips);
    this.WidgetViewDefinitions.push(ActorsNeedsActions);
    this.WidgetViewDefinitions.push(PendingPriceOffers);
    this.WidgetViewDefinitions.push(MostTruckTypeUsed);
    this.WidgetViewDefinitions.push(ActorNextDocDueDate);
    this.WidgetViewDefinitions.push(ActorNextInvoiceDueDate);
    this.WidgetViewDefinitions.push(NewInvoicesVsPaidInvoices);
    this.WidgetViewDefinitions.push(ActorsPaidInvoiceVsClaimedInvoice);
  }
}
