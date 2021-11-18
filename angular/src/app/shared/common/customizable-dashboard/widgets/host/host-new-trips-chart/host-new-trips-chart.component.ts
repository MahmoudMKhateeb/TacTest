import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-host-new-trips-chart',
  templateUrl: './host-new-trips-chart.component.html',
  styles: [],
})
export class HostNewTripsChartComponent implements OnInit {
  x: string[];
  y: number[];

  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}
  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.x = [];
    this.y = [];

    this._hostDashboardServiceProxy.getNewTripsCountPerMonth().subscribe((result) => {
      result.forEach((element) => {
        this.x.push(element.month);
        this.y.push(element.count);
      });
    });
    this.chartOptions = {
      series: [
        {
          name: 'Trips',
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
