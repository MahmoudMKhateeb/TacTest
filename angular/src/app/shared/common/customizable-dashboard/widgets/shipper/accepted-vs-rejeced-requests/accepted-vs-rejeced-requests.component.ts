import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-accepted-vs-rejeced-requests',
  templateUrl: './accepted-vs-rejeced-requests.component.html',
  styleUrls: ['./accepted-vs-rejeced-requests.component.css'],
})
export class AcceptedVsRejecedRequestsComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  loading = false;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    this.loading = true;
    this._shipperDashboardServiceProxy
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
              name: 'Accepted',
              data: result.acceptedOffers,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: 'Rejected',
              data: result.rejectedOffers,
              color: '#b5b5c3',
            },
          ],
          chart: {
            type: 'bar',
            width: 400,
            height: 200,
          },
          xaxis: {
            type: 'category',
          },
        };
        this.loading = false;
      });
  }
}
