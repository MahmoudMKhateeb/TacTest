import { Component, Injectable, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-accepted-vs-rejeced-requests',
  templateUrl: './accepted-vs-rejeced-requests.component.html',
  styleUrls: ['./accepted-vs-rejeced-requests.component.css'],
})
export class AcceptedVsRejecedRequestsComponent implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  constructor() {}

  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'Accepted',
          data: [20, 14, 28, 11, 42, 30, 50, 11, 32, 14, 28, 11],
        },
        {
          name: 'Rejected',
          data: [34, 52, 41, 28, 51, 11, 32, 45, 32, 42, 109, 100],
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
