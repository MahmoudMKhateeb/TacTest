import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-host-good-types-usage-chart',
  templateUrl: './host-good-types-usage-chart.component.html',
  styles: [],
})
export class HostGoodTypesUsageChartComponent implements OnInit {
  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'Accounts',
          data: [12, 14, 15, 26, 29, 42, 45, 58, 81, 83, 86, 94],
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
