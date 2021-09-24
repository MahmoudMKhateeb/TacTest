import { Component, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-number-of-total-completed-trips',
  templateUrl: './number-of-total-completed-trips.component.html',
  styleUrls: ['./number-of-total-completed-trips.component.css'],
})
export class NumberOfTotalCompletedTripsComponent implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  constructor() {}

  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'Trips',
          data: [10, 41, 35, 51, 49, 62, 69, 91, 148],
        },
      ],
      chart: {
        height: 350,
        type: 'line',
        zoom: {
          enabled: false,
        },
      },
      dataLabels: {
        enabled: false,
      },
      stroke: {
        curve: 'straight',
      },
      grid: {
        row: {
          colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
          opacity: 0.5,
        },
      },
      xaxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'],
      },
    };
  }
}
