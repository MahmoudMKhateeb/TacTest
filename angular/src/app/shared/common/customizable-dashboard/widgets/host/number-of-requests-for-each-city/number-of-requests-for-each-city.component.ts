import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

import { ApexAxisChartSeries, ApexTitleSubtitle, ApexDataLabels, ApexChart, ApexPlotOptions, ChartComponent } from 'ng-apexcharts';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  title: ApexTitleSubtitle;
  plotOptions: ApexPlotOptions;
};

@Component({
  selector: 'app-number-of-requests-for-each-city',
  templateUrl: './number-of-requests-for-each-city.component.html',
  styleUrls: ['./number-of-requests-for-each-city.component.css'],
})
export class NumberOfRequestsForEachCityComponent extends AppComponentBase implements OnInit {
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
    this.chartOptions = {
      series: [
        {
          name: 'Jan',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Feb',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Mar',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Apr',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'May',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Jun',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Jul',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Aug',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
        {
          name: 'Sep',
          data: this.generateData(20, {
            min: -30,
            max: 55,
          }),
        },
      ],
      chart: {
        height: 350,
        type: 'heatmap',
      },
      plotOptions: {
        heatmap: {
          shadeIntensity: 0.5,
          colorScale: {
            ranges: [
              {
                from: -30,
                to: 5,
                name: 'low',
                color: '#00A100',
              },
              {
                from: 6,
                to: 20,
                name: 'medium',
                color: '#128FD9',
              },
              {
                from: 21,
                to: 45,
                name: 'high',
                color: '#FFB200',
              },
              {
                from: 46,
                to: 55,
                name: 'extreme',
                color: '#FF0000',
              },
            ],
          },
        },
      },
      dataLabels: {
        enabled: false,
      },
      title: {
        text: 'HeatMap Chart with Color Range',
      },
    };
  }

  ngOnInit(): void {}

  public generateData(count, yrange) {
    var i = 0;
    var series = [];
    while (i < count) {
      var x = 'w' + (i + 1).toString();
      var y = Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;

      series.push({
        x: x,
        y: y,
      });
      i++;
    }
    return series;
  }
}
