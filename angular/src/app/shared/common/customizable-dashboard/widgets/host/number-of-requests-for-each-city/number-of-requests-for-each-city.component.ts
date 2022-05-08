import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as ApexCharts from 'apexcharts';

import { ApexAxisChartSeries, ApexTitleSubtitle, ApexDataLabels, ApexChart, ApexPlotOptions, ChartComponent } from 'ng-apexcharts';
import { finalize } from 'rxjs/operators';

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
  y: number[];
  series1: any[];

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.y = [];
    this.series1 = [];
    this.loading = true;

    this._hostDashboardServiceProxy
      .getNumberOfRequestsForEachCity()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          this.series1.push({
            name: element.cityName,
            data: this.generateData(element.numberOfRequests, {
              min: element.minimumValueOfRequests,
              max: element.maximumValueOfRequests,
            }),
          });
        });

        this.chartOptions = {
          series: this.series1,
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
                    color: '#dc2c34',
                  },
                  {
                    from: 6,
                    to: 20,
                    name: 'medium',
                    color: '#b5b5c3',
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
        this.loading = false;
      });
  }

  public generateData(count, yrange) {
    var i = 0;
    var series = [];
    while (i < count) {
      var x = '';

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
