import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ChartOptionsBars } from '../../ApexInterfaces';

@Component({
  selector: 'app-host-route-type-usage-chart',
  templateUrl: './host-route-type-usage-chart.component.html',
  styles: [],
})
export class HostRouteTypeUsageChartComponent extends AppComponentBase implements OnInit {
  routes: string[];
  counts: number[];
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.getData();
  }

  getData() {
    this.routes = [];
    this.counts = [];
    this.loading = true;
    this._hostDashboardServiceProxy
      .getRouteTypeCountPerMonth()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          this.routes.push(element.routeType);
          this.counts.push(element.availableRouteTypesCount);
        });
        this.chartOptions = {
          series: [
            {
              name: 'Shipping Requests',
              data: this.counts,
              color: '#b10303',
            },
          ],
          chart: {
            type: 'bar',
            height: 350,
          },
          plotOptions: {
            bar: {
              horizontal: false,
              columnWidth: '55%',
            },
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            show: true,
            width: 2,
            colors: ['transparent'],
          },
          xaxis: {
            categories: this.routes,
          },

          fill: {
            opacity: 1,
          },
        };
        this.loading = false;
      });
  }
}
