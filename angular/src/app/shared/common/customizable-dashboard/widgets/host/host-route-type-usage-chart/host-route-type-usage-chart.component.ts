import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { ChartOptionsBars } from '../../ApexInterfaces';

@Component({
  selector: 'app-host-route-type-usage-chart',
  templateUrl: './host-route-type-usage-chart.component.html',
  styles: [],
})
export class HostRouteTypeUsageChartComponent implements OnInit {
  x: string[];
  y: number[];

  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.x = [];
    this.y = [];

    this._hostDashboardServiceProxy.getRouteTypeCountPerMonth().subscribe((result) => {
      result.forEach((element) => {
        this.x.push(element.routeType);
        this.y.push(element.availableRouteTypesCount);
      });
    });
    this.chartOptions = {
      series: [
        {
          name: 'Shipping Requests',
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
  }
}
