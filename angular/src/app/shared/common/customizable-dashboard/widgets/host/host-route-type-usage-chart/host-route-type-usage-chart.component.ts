import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexChart, ApexNonAxisChartSeries, ApexResponsive, ApexTooltip } from '@node_modules/ng-apexcharts';
import { ApexLegend } from '@node_modules/ng-apexcharts/lib/model/apex-types';
export interface ChartOptions {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
}

@Component({
  selector: 'app-host-route-type-usage-chart',
  templateUrl: './host-route-type-usage-chart.component.html',
  styleUrls: ['./host-route-type-usage-chart.component.scss'],
})
export class HostRouteTypeUsageChartComponent extends AppComponentBase implements OnInit {
  routes: string[] = [];
  counts: number[] = [];
  total = 0;
  loading = false;
  legend: ApexLegend = {};
  colors: string[] = [];
  tooltip: ApexTooltip = {
    custom: ({ series, seriesIndex, dataPointIndex, w }) => {
      const percentage = (series[seriesIndex] / this.total) * 100;
      const fixedPercentage = parseFloat(percentage.toFixed(2));
      return `<div class="arrow_box" style="padding: 0.6rem; background: ${this.colors[seriesIndex]}"><span>${fixedPercentage}% ${this.l(
        this.routes[seriesIndex]
      )} </span></div>`;
    },
    fillSeriesColor: true,
  };
  public chartOptions: Partial<ChartOptions>;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.routes = [];
    this.counts = [];
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getRouteTypeCount()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        const that = this;
        result.forEach((element, i) => {
          this.total += element.availableRouteTypesCount;
          this.routes.push(element.routeType);
          this.counts.push(element.availableRouteTypesCount);
          this.colors.push(i % 2 === 0 ? '#da1a32' : '#231f20');
        });
        this.chartOptions = {
          series: this.counts,
          chart: {
            type: 'donut',
            width: '70%',
            height: '100%',
          },
          labels: this.routes,
          responsive: [
            {
              breakpoint: 480,
              options: {
                chart: {
                  width: 200,
                },
              },
            },
          ],
        };
        this.legend = {
          show: false,
          formatter: function (legendName: string, opts?: any) {
            return that.routes[opts.seriesIndex] + '...' + result[opts.seriesIndex].availableRouteTypesCount;
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
