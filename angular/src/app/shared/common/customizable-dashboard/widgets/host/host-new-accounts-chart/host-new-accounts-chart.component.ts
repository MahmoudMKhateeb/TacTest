import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-host-new-accounts-chart',
  templateUrl: './host-new-accounts-chart.component.html',
  styles: [],
})
export class HostNewAccountsChartComponent implements OnInit {
  constructor() {}
  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'Accounts',
          data: [2, 5, 14, 38, 42, 55, 56, 61, 76, 80],
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
        categories: ['Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'],
      },

      fill: {
        opacity: 1,
      },
    };
  }
}
