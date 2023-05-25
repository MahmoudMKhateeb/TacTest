import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from '@node_modules/primeng/chart';
import { NgApexchartsModule } from '@node_modules/ng-apexcharts';
import { CompletedTripsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/completed-trips-widget/completed-trips-widget.component';
import { CompletedTripVsPodComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/completed-trip-vs-pod/completed-trip-vs-pod.component';
import { AcceptedVsRejecedRequestsComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/accepted-vs-rejeced-requests/accepted-vs-rejeced-requests.component';
import { InvoicesVsPaidInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/invoices-vs-paid-invoices/invoices-vs-paid-invoices.component';
import { MostWorkedWithCarriersComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-worked-with-carriers/most-worked-with-carriers.component';
import { RequestsInMarketPlaceComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/requests-in-market-place/requests-in-market-place.component';
import { TableModule } from '@node_modules/primeng/table';
import { NextInvoiceFrequancyDateComponent } from './shipper/next-invoice-frequancy-date/next-invoice-frequancy-date.component';
import { InvoiceDueDateComponent } from './shipper/invoice-due-date/invoice-due-date.component';
import { ShipperDueDateInDaysComponent } from './shared_widgets/shipper-due-date-in-days/shipper-due-date-in-days.component';
import { MostUsedOriginComponent } from './shipper/most-used-origin/most-used-origin.component';
import { MostUsedDestinationsComponent } from './shipper/most-used-destinations/most-used-destinations.component';
import { TrackingMapComponent } from './shared_widgets/tracking-map/tracking-map.component';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { AgmDirectionModule } from '@node_modules/agm-direction';
import { AcceptedVsRejectedPricingComponent } from './carrier/accepted-vs-rejected-pricing/accepted-vs-rejected-pricing.component';
import { TucksActivityComponent } from './carrier/tucks-activity/tucks-activity.component';
import { DriversActivityComponent } from './carrier/drivers-activity/drivers-activity.component';
import { MostUsedppComponent } from './carrier/most-usedpp/most-usedpp.component';
import { MostUsedVasesComponent } from './carrier/most-used-vases/most-used-vases.component';
import { MostWorkedWithShippersComponent } from './carrier/most-worked-with-shippers/most-worked-with-shippers.component';
import { CarrierNextInvoiceFrequenctEndComponent } from './carrier/carrier-next-invoice-frequenct-end/carrier-next-invoice-frequenct-end.component';
import { NumberOfRegesterdTrucksComponent } from './host/number-of-regesterd-trucks/number-of-regesterd-trucks.component';
import { NumberOfRegesterdDriversComponent } from './host/number-of-regesterd-drivers/number-of-regesterd-drivers.component';
import { NumberOfRegesterdShippersComponent } from './host/number-of-regesterd-shippers/number-of-regesterd-shippers.component';
import { OnGoingTripsComponent } from './host/on-going-trips/on-going-trips.component';
import { DeleverdTripsComponent } from './host/deleverd-trips/deleverd-trips.component';
import { NumberOfRegesterdCarriersComponent } from './host/number-of-regesterd-carriers/number-of-regesterd-carriers.component';
import { HostNewAccountsChartComponent } from './host/host-new-accounts-chart/host-new-accounts-chart.component';
import { HostNewTripsChartComponent } from './host/host-new-trips-chart/host-new-trips-chart.component';
import { HostTruckTypeUsageChartComponent } from './host/host-truck-type-usage-chart/host-truck-type-usage-chart.component';
import { HostGoodTypesUsageChartComponent } from './host/host-good-types-usage-chart/host-good-types-usage-chart.component';
import { HostRouteTypeUsageChartComponent } from './host/host-route-type-usage-chart/host-route-type-usage-chart.component';
import { HostRquestPricingMeterComponent } from './host/host-rquest-pricing-meter/host-rquest-pricing-meter.component';
import { HostRquestAcceptanceMeterComponent } from './host/host-rquest-acceptance-meter/host-rquest-acceptance-meter.component';
import { HostInvoicesMeterComponent } from './host/host-invoices-meter/host-invoices-meter.component';
import { TopThreeShippersHaveRequestsComponent } from './host/top-three-shippers-have-requests/top-three-shippers-have-requests.component';
import { TopThreeCarriersHaveRequestsComponent } from './host/top-three-carriers-have-requests/top-three-carriers-have-requests.component';
import { TopRatedCarriersComponent } from './host/top-rated-carriers/top-rated-carriers.component';
import { TopRatedShippersComponent } from './host/top-rated-shippers/top-rated-shippers.component';
import { WorstRatedShippersComponent } from './host/worst-rated-shippers/worst-rated-shippers.component';
import { WorstRatedCarriersComponent } from './host/worst-rated-carriers/worst-rated-carriers.component';
import { NumberOfRequestsForEachCityComponent } from './host/number-of-requests-for-each-city/number-of-requests-for-each-city.component';
import { UnpricedRequestsInMarketplaceComponent } from './host/unpriced-requests-in-marketplace/unpriced-requests-in-marketplace.component';
import { PaginatorModule } from '@node_modules/primeng/paginator';
import { UtilsModule } from '@shared/utils/utils.module';
import { BsDatepickerModule } from '@node_modules/ngx-bootstrap/datepicker';
import { NgbRatingModule } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { AngularFireModule } from '@angular/fire/compat';
import { environment } from '../../../../../environments/environment';
import { AngularFirestoreModule } from '@angular/fire/compat/firestore';
import { RouterModule } from '@angular/router';
import { CarrierComplitedTripsWidgetComponent } from './carrier/carrier-complited-trips-widget/carrier-complited-trips-widget.component';
import { CarrierAcceptedVsRejectdRequestsComponent } from './carrier/carrier-accepted-vs-rejectd-requests/carrier-accepted-vs-rejectd-requests.component';
import { CarrierInvoicesDetailsWidgetComponent } from './carrier/carrier-invoices-details-widget/carrier-invoices-details-widget.component';
import { ProjectPerformanceComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/project-performance/project-performance.component';
import { TruckPerformanceChartComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/truck-performance/truck-performance-chart/truck-performance-chart.component';
import { DxDateBoxModule } from '@node_modules/devextreme-angular/ui/date-box';
import { DxSelectBoxModule } from '@node_modules/devextreme-angular/ui/select-box';
import { CountersWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/counters-widget/counters-widget.component';
import { UpcomingTripsWidgetsComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/upcoming-trips-widgets/upcoming-trips-widgets.component';
import { NeedsActionWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/needs-action-widget/needs-action-widget.component';
import { NewOffersWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/new-offers-widget/new-offers-widget.component';
import { ShipperDashboardComponent } from '@app/shared/common/customizable-dashboard/shipper-dashboard/shipper-dashboard.component';
import { CarrierDashboardComponent } from '@app/shared/common/customizable-dashboard/carrier-dashboard/carrier-dashboard.component';
import { NewDirectRequestsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shared_widgets/new-direct-requests-widget/new-direct-requests-widget.component';
import { TabsModule } from '@node_modules/ngx-bootstrap/tabs';
import { FromToComponent } from '@app/shared/common/from-to/from-to.component';
import { DriverTucksActivityComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/driver-tucks-activity/driver-tucks-activity.component';
import { NewActorsThisMonthComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/new-actors-this-month/new-actors-this-month.component';
import { NumberOfActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/number-of-actors/number-of-actors.component';
import { MostActiveActorComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-active-actor/most-active-actor.component';
import { MostActiveActorShipperComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-active-actor-shipper/most-active-actor-shipper.component';
import { MostActiveActorCarrierComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-active-actor-carrier/most-active-actor-carrier.component';
import { NumberOfActiveActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/number-of-active-actors/number-of-active-actors.component';
import { MostUsedDestinationsActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-used-destinations-actors/most-used-destinations-actors.component';
import { MostUsedOriginsActorsComponent } from '@app/shared/common/customizable-dashboard/widgets/broker/most-used-origins-actors/most-used-origins-actors.component';
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
import { AgmMarkerClustererModule } from '@agm/markerclusterer';
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
import { MakeScrollableModule } from '@app/shared/common/make-scrollable-directive/make-scrollable.module';

const widgets = [
  CompletedTripsWidgetComponent,
  CompletedTripVsPodComponent,
  AcceptedVsRejecedRequestsComponent,
  InvoicesVsPaidInvoicesComponent,
  MostWorkedWithCarriersComponent,
  RequestsInMarketPlaceComponent,
  NextInvoiceFrequancyDateComponent,
  InvoiceDueDateComponent,
  ShipperDueDateInDaysComponent,
  MostUsedOriginComponent,
  MostUsedDestinationsComponent,
  TrackingMapComponent,
  AcceptedVsRejectedPricingComponent,
  TucksActivityComponent,
  DriversActivityComponent,
  MostUsedppComponent,
  MostUsedVasesComponent,
  MostWorkedWithShippersComponent,
  CarrierNextInvoiceFrequenctEndComponent,
  NumberOfRegesterdTrucksComponent,
  NumberOfRegesterdDriversComponent,
  NumberOfRegesterdShippersComponent,
  OnGoingTripsComponent,
  DeleverdTripsComponent,
  NumberOfRegesterdCarriersComponent,
  HostNewAccountsChartComponent,
  HostNewTripsChartComponent,
  HostTruckTypeUsageChartComponent,
  HostGoodTypesUsageChartComponent,
  HostRouteTypeUsageChartComponent,
  HostRquestPricingMeterComponent,
  HostRquestAcceptanceMeterComponent,
  HostInvoicesMeterComponent,
  TopThreeShippersHaveRequestsComponent,
  TopThreeCarriersHaveRequestsComponent,
  TopRatedCarriersComponent,
  TopRatedShippersComponent,
  WorstRatedShippersComponent,
  WorstRatedCarriersComponent,
  NumberOfRequestsForEachCityComponent,
  UnpricedRequestsInMarketplaceComponent,
  CarrierComplitedTripsWidgetComponent,
  CarrierAcceptedVsRejectdRequestsComponent,
  CarrierInvoicesDetailsWidgetComponent,
  ProjectPerformanceComponent,
  TruckPerformanceChartComponent,
  CountersWidgetComponent,
  UpcomingTripsWidgetsComponent,
  NeedsActionWidgetComponent,
  NewOffersWidgetComponent,
  ShipperDashboardComponent,
  CarrierDashboardComponent,
  NewDirectRequestsWidgetComponent,
  FromToComponent,
  DriverTucksActivityComponent,
  NewActorsThisMonthComponent,
  NumberOfActorsComponent,
  MostActiveActorComponent,
  MostActiveActorShipperComponent,
  MostActiveActorCarrierComponent,
  NumberOfActiveActorsComponent,
  MostUsedOriginsActorsComponent,
  MostUsedDestinationsActorsComponent,
  ActorUpcomingTripsComponent,
  ActorNeedsActionsComponent,
  ActorPendingPriceOffersComponent,
  ActorMostTruckTypeUsedComponent,
  ActorNextDocDueDateComponent,
  ActorNextInvoiceDueDateComponent,
  ActorNewInvoiceVsPaidInvoiceComponent,
  ActorPaidInvoiceVsClaimedInvoiceComponent,
  NumberOfRegisteredCompaniesComponent,
  NumberOfDriversAndTrucksComponent,
  TopRatedShippersAndCarriersComponent,
  NormalRequestsVSDedicatedRequestsComponent,
  NumberOfTripsComponent,
  NumberOfSaasTripsComponent,
  NumberOfTruckAggregationTripsComponent,
  NumberOfTruckAggregationTripsVsSaasTripsComponent,
  TopWorstRatedPerTripComponent,
  PaidInvoicesBeforeDueDateComponent,
  CostVsSellingVsProfitOfSaasTripsComponent,
  OverallTotalAmountPerAllTripsComponent,
  TruckAggregationInvoicesComponent,
  SaasInvoicesComponent,
  GoodTypesUsageComponent,
  CostVsSellingVsProfitOfTruckAggregationTripsComponent,
  NewRegisteredCompaniesComponent,
];
@NgModule({
  declarations: [...widgets],
  imports: [
    CommonModule,
    ChartModule,
    NgApexchartsModule,
    TableModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyDKKZqDW_xX5azTqBV2oXSb6P3nwCAzOpw',
    }),
    AgmMarkerClustererModule,
    AgmDirectionModule,
    PaginatorModule,
    UtilsModule,
    BsDatepickerModule,
    NgbRatingModule,
    AngularFireModule.initializeApp(environment.firebase),
    TabsModule.forRoot(),
    AngularFirestoreModule,
    RouterModule,
    DxSelectBoxModule,
    DxDateBoxModule,
    MakeScrollableModule,
  ],
  entryComponents: [...widgets],
  exports: [...widgets],
})
export class WidgetsModule {}
