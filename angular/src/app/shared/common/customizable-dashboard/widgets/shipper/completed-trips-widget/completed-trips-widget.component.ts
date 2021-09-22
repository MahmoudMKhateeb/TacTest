import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { WidgetComponentBase } from '@app/shared/common/customizable-dashboard/widgets/widget-component-base';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-completed-trips-widget',
  templateUrl: './completed-trips-widget.component.html',
  styleUrls: ['./completed-trips-widget.component.css'],
})
export class CompletedTripsWidgetComponent extends WidgetComponentBase {
  public chartOptions: Partial<ChartOptions>;

  constructor(injector: Injector) {
    super(injector);
    this.chartOptions = {
      series: [
        {
          name: 'Trips',
          data: [10, 41, 35, 51, 49, 62, 69, 91, 148],
          color: '#801e1e',
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
