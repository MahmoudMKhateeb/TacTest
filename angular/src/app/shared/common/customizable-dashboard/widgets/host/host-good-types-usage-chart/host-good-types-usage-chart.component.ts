import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-host-good-types-usage-chart',
  templateUrl: './host-good-types-usage-chart.component.html',
  styles: [],
})
export class HostGoodTypesUsageChartComponent extends AppComponentBase implements OnInit {
  x: string[];
  y: number[];
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.getData();
  }

  getData() {
    this.x = [];
    this.y = [];
    this.loading = true;
    this._hostDashboardServiceProxy
      .getGoodTypeCountPerMonth()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          this.x.push(element.goodType);
          this.y.push(element.availableGoodTypesCount);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Category',
              data: this.y,
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
            categories: this.x,
          },

          fill: {
            opacity: 1,
          },
        };
        this.loading = false;
      });
  }
}
