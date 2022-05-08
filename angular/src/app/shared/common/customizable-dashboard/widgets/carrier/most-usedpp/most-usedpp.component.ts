import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-most-usedpp',
  templateUrl: './most-usedpp.component.html',
  styleUrls: ['./most-usedpp.component.css'],
})
export class MostUsedppComponent implements OnInit {
  constructor() {}

  public chartOptions: Partial<ChartOptionsBars>;

  ngOnInit() {
    this.chartOptions = {
      series: [
        {
          name: 'UseCount',
          data: [44, 55, 57, 56, 61, 58, 63, 60, 66],
          color: 'rgba(187, 41, 41, 0.847)',
        },
      ],
      chart: {
        type: 'bar',
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

      fill: {
        opacity: 1,
      },
    };
  }
}
