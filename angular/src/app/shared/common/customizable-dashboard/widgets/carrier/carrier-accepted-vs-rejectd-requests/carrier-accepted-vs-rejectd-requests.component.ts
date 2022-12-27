import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-carrier-accepted-vs-rejectd-requests',
  templateUrl: './carrier-accepted-vs-rejectd-requests.component.html',
  styleUrls: ['./carrier-accepted-vs-rejectd-requests.component.css'],
})
export class CarrierAcceptedVsRejectdRequestsComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  loading = false;

  constructor(injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    this.loading = true;
    this._carrierDashboardServiceProxy
      .getAcceptedAndRejectedRequests()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [
            {
              name: this.l('Accepted'),
              data: result.acceptedOffers,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: this.l('Rejected'),
              data: result.rejectedOffers,
            },
          ],
          chart: {
            type: 'area',
            width: 400,
            height: 250,
          },
          xaxis: {
            type: 'category',
          },
        };
        (this.chartOptions.chart.locales as any[]) = [
          {
            name: 'en',
            options: {
              toolbar: {
                exportToPNG: this.l('Download') + ' PNG',
                exportToSVG: this.l('Download') + ' SVG',
                exportToCSV: this.l('Download') + ' CSV',
              },
            },
          },
        ];
        this.loading = false;
      });
  }
}
