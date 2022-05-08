import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-host-truck-type-usage-chart',
  templateUrl: './host-truck-type-usage-chart.component.html',
  styles: [],
})
export class HostTruckTypeUsageChartComponent extends AppComponentBase implements OnInit {
  types: string[];
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
    this.types = [];
    this.counts = [];
    this.loading = true;

    this._hostDashboardServiceProxy
      .getTrucksTypeCount()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          this.types.push(element.truckType);
          this.counts.push(element.availableTrucksCount);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Requests',
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
            categories: this.types,
          },

          fill: {
            opacity: 1,
          },
          tooltip: {
            y: {
              formatter: function (val) {
                return val.toFixed(0);
              },
            },
          },
        };

        this.loading = false;
      });
  }
}
