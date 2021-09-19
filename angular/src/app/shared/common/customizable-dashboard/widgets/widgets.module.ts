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

@NgModule({
  declarations: [
    CompletedTripsWidgetComponent,
    CompletedTripVsPodComponent,
    AcceptedVsRejecedRequestsComponent,
    InvoicesVsPaidInvoicesComponent,
    MostWorkedWithCarriersComponent,
    RequestsInMarketPlaceComponent,
  ],
  imports: [CommonModule, ChartModule, NgApexchartsModule, TableModule],
  entryComponents: [CompletedTripsWidgetComponent, AcceptedVsRejecedRequestsComponent, CompletedTripVsPodComponent],
})
export class WidgetsModule {}
