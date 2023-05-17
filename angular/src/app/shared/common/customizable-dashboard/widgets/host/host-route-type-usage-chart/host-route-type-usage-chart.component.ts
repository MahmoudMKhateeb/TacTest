import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
// import { ChartOptionsBars } from '../../ApexInterfaces';

import { ApexChart, ApexNonAxisChartSeries, ApexResponsive } from '@node_modules/ng-apexcharts';
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
      return `<div class="arrow_box" style="padding: 0.6rem; background: ${this.colors[seriesIndex]}"><span>${series[seriesIndex]}% ${this.l(
        this.routes[seriesIndex]
      )} </span></div>`;
    },
    fillSeriesColor: true,
  };
  public chartOptions: Partial<ChartOptions>;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.routes = [];
    this.counts = [];
    this.loading = true;
    this._hostDashboardServiceProxy
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
                // legend: {
                //   position: 'bottom',
                // },
              },
            },
          ],
        };
        this.legend = {
          show: false,
          formatter: function (legendName: string, opts?: any) {
            console.log('legendName', legendName);
            console.log('opts', opts);
            return that.routes[opts.seriesIndex] + '...' + result[opts.seriesIndex].availableRouteTypesCount;
          },
        };
        // this.chartOptions = {
        //   series: [
        //     {
        //       name: 'Shipping Requests',
        //       data: this.counts,
        //       color: '#b10303',
        //     },
        //   ],
        //   chart: {
        //     type: 'bar',
        //     height: 350,
        //   },
        //   plotOptions: {
        //     bar: {
        //       horizontal: false,
        //       columnWidth: '55%',
        //     },
        //   },
        //   dataLabels: {
        //     enabled: false,
        //   },
        //   stroke: {
        //     show: true,
        //     width: 2,
        //     colors: ['transparent'],
        //   },
        //   xaxis: {
        //     categories: this.routes,
        //   },
        //   yaxis: {
        //     opposite: this.isRtl,
        //   },
        //   tooltip: {
        //     y: {
        //       formatter: function (val) {
        //         return val.toFixed(0);
        //       },
        //     },
        //   },
        //   fill: {
        //     opacity: 1,
        //   },
        // };
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
