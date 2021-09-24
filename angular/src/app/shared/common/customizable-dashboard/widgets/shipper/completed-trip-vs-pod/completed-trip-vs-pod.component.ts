import { Component, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-completed-trip-vs-pod',
  templateUrl: './completed-trip-vs-pod.component.html',
  styleUrls: ['./completed-trip-vs-pod.component.css'],
})
export class CompletedTripVsPodComponent implements OnInit {
  constructor() {}
  public chartOptions: Partial<ChartOptions>;

  ngOnInit(): void {
    this.chartOptions = {
      series: [
        {
          name: 'Completed',
          data: [31, 40, 28, 51, 42, 109, 100, 11, 32, 45, 32, 34],
        },
        {
          name: 'Pod',
          data: [11, 32, 45, 32, 34, 52, 41, 28, 51, 42, 109, 100],
        },
      ],
      chart: {
        height: 350,
        type: 'area',
      },
      dataLabels: {
        enabled: false,
      },
      stroke: {
        curve: 'smooth',
      },
      xaxis: {
        type: 'category',
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'],
      },
      tooltip: {
        x: {
          format: 'dd/MM/yy',
        },
      },
    };
  }
}
