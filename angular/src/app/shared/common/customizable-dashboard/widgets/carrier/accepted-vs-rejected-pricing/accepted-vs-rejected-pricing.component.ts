import { Component, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-accepted-vs-rejected-pricing',
  templateUrl: './accepted-vs-rejected-pricing.component.html',
  styleUrls: ['./accepted-vs-rejected-pricing.component.css'],
})
export class AcceptedVsRejectedPricingComponent implements OnInit {
  public chartOptions: Partial<ChartOptions>;

  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'Rejected',
          data: [34, 52, 41, 28, 51, 11, 32, 45, 32, 42, 109, 100],
          color: '#78786c',
        },
        {
          name: 'Accepted',
          data: [20, 14, 28, 11, 42, 30, 50, 11, 32, 14, 28, 11],
          color: '#b30000',
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
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      },
      tooltip: {
        x: {
          format: 'dd/MM/yy',
        },
      },
    };
  }
}
