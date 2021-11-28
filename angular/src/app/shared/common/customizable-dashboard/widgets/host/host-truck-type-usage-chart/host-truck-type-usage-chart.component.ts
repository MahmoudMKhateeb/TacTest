import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-host-truck-type-usage-chart',
  templateUrl: './host-truck-type-usage-chart.component.html',
  styles: [],
})
export class HostTruckTypeUsageChartComponent implements OnInit {
  x: string[];
  y: number[];

  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.x = [];
    this.y = [];

    this._hostDashboardServiceProxy.getTrucksTypeCount().subscribe((result) => {
      result.forEach((element) => {
        this.x.push(element.truckType);
        this.y.push(element.availableTrucksCount);
      });
    });

    this.chartOptions = {
      series: [
        {
          name: 'Requests',
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
