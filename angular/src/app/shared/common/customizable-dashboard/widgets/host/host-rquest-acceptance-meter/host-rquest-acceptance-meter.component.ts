import { Component, OnInit } from '@angular/core';
import { MeterCharts } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-host-rquest-acceptance-meter',
  templateUrl: './host-rquest-acceptance-meter.component.html',
  styles: [],
})
export class HostRquestAcceptanceMeterComponent {
  public chartOptions: Partial<MeterCharts>;

  constructor() {
    this.chartOptions = {
      series: [67],

      chart: {
        height: 350,
        type: 'radialBar',
        offsetY: -10,
      },

      plotOptions: {
        radialBar: {
          startAngle: -135,
          endAngle: 135,
          dataLabels: {
            name: {
              fontSize: '16px',
              color: undefined,
              offsetY: 120,
            },
            value: {
              offsetY: 76,
              fontSize: '22px',
              color: undefined,
            },
          },
        },
      },
      fill: {
        type: 'gradient',
        gradient: {
          shade: 'dark',
          shadeIntensity: 0.15,
          inverseColors: false,
          opacityFrom: 1,
          opacityTo: 1,
          stops: [0, 50, 65, 91],
        },
      },
      stroke: {
        dashArray: 4,
      },
      labels: ['Median Ratio'],
    };
  }
}
