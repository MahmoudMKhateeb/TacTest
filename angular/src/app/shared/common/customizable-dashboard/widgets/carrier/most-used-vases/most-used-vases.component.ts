import { Component, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';

@Component({
  selector: 'app-most-used-vases',
  templateUrl: './most-used-vases.component.html',
  styleUrls: ['./most-used-vases.component.css'],
})
export class MostUsedVasesComponent implements OnInit {
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
        categories: ['VAS', 'VAS2', 'VAS3', 'VAS4', 'VAS5', 'VAS6', 'VAS8', 'VAS9', 'VAS10'],
      },

      fill: {
        opacity: 1,
      },
    };
  }
}
