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
import { TrackingMapComponent } from './shipper/tracking-map/tracking-map.component';
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

@NgModule({
  declarations: [
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
  ],
  imports: [
    CommonModule,
    ChartModule,
    NgApexchartsModule,
    TableModule,
    AgmCoreModule,
    AgmDirectionModule,
    PaginatorModule,
    UtilsModule,
    BsDatepickerModule,
    NgbRatingModule,
    AngularFireModule.initializeApp(environment.firebase),
    AngularFirestoreModule,
    RouterModule,
  ],
  entryComponents: [
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
  ],
})
export class WidgetsModule {}
