import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-completed-trip-vs-pod',
  templateUrl: './completed-trip-vs-pod.component.html',
  styleUrls: ['./completed-trip-vs-pod.component.css'],
})
export class CompletedTripVsPodComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  loading = false;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getTrips();
  }

  getTrips() {
    this.loading = true;

    this._shipperDashboardServiceProxy
      .getCompletedTripVsPod()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [
            {
              name: 'Completed',
              data: result.completedTrips,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: 'Pod',
              data: result.podTrips,
              color: '#b5b5c3',
            },
          ],
          chart: {
            height: 350,
            type: 'area',
          },
          xaxis: {
            type: 'category',
          },
        };
        this.loading = false;
      });
  }
}
