import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-host-truck-type-usage-chart',
  templateUrl: './host-truck-type-usage-chart.component.html',
  styles: [],
})
export class HostTruckTypeUsageChartComponent implements OnInit {
  constructor() {}

  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'Trips',
          data: [14, 15, 36, 63, 64, 73, 85, 89, 91, 93, 9],
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
