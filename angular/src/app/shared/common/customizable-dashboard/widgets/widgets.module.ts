import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from '@node_modules/primeng/chart';
import { NgApexchartsModule } from '@node_modules/ng-apexcharts';
import { CompletedTripsWidgetComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/completed-trips-widget/completed-trips-widget.component';
import { CompletedTripVsPodComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/completed-trip-vs-pod/completed-trip-vs-pod.component';
import { AcceptedVsRejecedRequestsComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/accepted-vs-rejeced-requests/accepted-vs-rejeced-requests.component';
import { InvoicesVsPaidInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/invoices-vs-paid-invoices/invoices-vs-paid-invoices.component';
import { MostWorkedWithCarriersComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-worked-with-carriers/most-worked-with-carriers.component';
import { RequestsInMarketPlaceComponent } from '@app/shared/common/customizable-dashboard/widgets/shipper/requests-in-market-place/requests-in-market-place.component';
import { TableModule } from '@node_modules/primeng/table';
import { NextInvoiceFrequancyDateComponent } from './shipper/next-invoice-frequancy-date/next-invoice-frequancy-date.component';
import { InvoiceDueDateComponent } from './shipper/invoice-due-date/invoice-due-date.component';
import { ShipperDueDateInDaysComponent } from './shipper/shipper-due-date-in-days/shipper-due-date-in-days.component';
import { MostUsedOriginComponent } from './shipper/most-used-origin/most-used-origin.component';
import { MostUsedDestinationsComponent } from './shipper/most-used-destinations/most-used-destinations.component';
import { TrackingMapComponent } from './shipper/tracking-map/tracking-map.component';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { AgmDirectionModule } from '@node_modules/agm-direction';
import { NumberOfTotalCompletedTripsComponent } from './carrier/number-of-total-completed-trips/number-of-total-completed-trips.component';
import { CarrierInvoicesVsPaidInvoicesComponent } from '@app/shared/common/customizable-dashboard/widgets/carrier/invoices-vs-paid-invoices/invoices-vs-paid-invoices.component';
import { AcceptedVsRejectedPricingComponent } from './carrier/accepted-vs-rejected-pricing/accepted-vs-rejected-pricing.component';
import { TucksActivityComponent } from './carrier/tucks-activity/tucks-activity.component';
import { DriversActivityComponent } from './carrier/drivers-activity/drivers-activity.component';
import { MostUsedppComponent } from './carrier/most-usedpp/most-usedpp.component';
import { MostUsedVasesComponent } from './carrier/most-used-vases/most-used-vases.component';
import { MostWorkedWithShippersComponent } from './carrier/most-worked-with-shippers/most-worked-with-shippers.component';
import { CarrierNextInvoiceFrequenctEndComponent } from './carrier/carrier-next-invoice-frequenct-end/carrier-next-invoice-frequenct-end.component';
import { CarrierDocumentsDueDateinDaysComponent } from './carrier/carrier-documents-due-datein-days/carrier-documents-due-datein-days.component';

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
    NumberOfTotalCompletedTripsComponent,
    CarrierInvoicesVsPaidInvoicesComponent,
    AcceptedVsRejectedPricingComponent,
    TucksActivityComponent,
    DriversActivityComponent,
    MostUsedppComponent,
    MostUsedVasesComponent,
    MostWorkedWithShippersComponent,
    CarrierNextInvoiceFrequenctEndComponent,
    CarrierDocumentsDueDateinDaysComponent,
  ],
  imports: [CommonModule, ChartModule, NgApexchartsModule, TableModule, AgmCoreModule, AgmDirectionModule],
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
    NumberOfTotalCompletedTripsComponent,
    CarrierInvoicesVsPaidInvoicesComponent,
    AcceptedVsRejectedPricingComponent,
    TucksActivityComponent,
    DriversActivityComponent,
    MostUsedppComponent,
    MostUsedVasesComponent,
    MostWorkedWithShippersComponent,
    CarrierNextInvoiceFrequenctEndComponent,
    CarrierDocumentsDueDateinDaysComponent,
  ],
})
export class WidgetsModule {}
