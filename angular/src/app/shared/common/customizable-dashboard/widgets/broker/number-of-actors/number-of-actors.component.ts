import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ChartComponent } from '@node_modules/ng-apexcharts';

import { ApexLegend, ApexPlotOptions, ApexOptions } from '@node_modules/ng-apexcharts/lib/model/apex-types';
import { BrokerDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-actors',
  templateUrl: './number-of-actors.component.html',
  styleUrls: ['./number-of-actors.component.scss'],
})
export class NumberOfActorsComponent extends AppComponentBase implements OnInit {
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ApexOptions>;
  colors = ['#DC2434', '#000'];
  labels = ['Shippers', 'Carriers'];
  tooltip: ApexTooltip = {
    custom: ({ series, seriesIndex, dataPointIndex, w }) => {
      return `<div class="arrow_box" style="padding: 0.6rem; background: ${this.colors[seriesIndex]}"><span>${series[seriesIndex]}% ${this.labels[seriesIndex]} </span></div>`;
    },
    fillSeriesColor: true,
  };
  plotOptions: ApexPlotOptions = {
    pie: {},
  };
  legend: ApexLegend = {};

  actorsCount: number = 100;
  // percentage: number = 10;
  shipperCount: number = 60;
  carrierCount: number = 40;

  constructor(injector: Injector, private brokerDashboardServiceProxy: BrokerDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.fetchData();
  }

  private fetchData() {
    this.brokerDashboardServiceProxy.getNumbersOfActors().subscribe((res) => {
      this.actorsCount = res.totalActors;
      this.shipperCount = res.shipperActorPercentage;
      this.carrierCount = res.carrierActorPercentage;
      this.chartOptions = {
        series: [this.shipperCount, this.carrierCount] /* [44, 55, 13, 43, 22] */,
        chart: {
          type: 'donut',
          width: '100%',
          height: 150,
        },
        labels: this.labels /* ["Team A", "Team B", "Team C", "Team D", "Team E"] */,
        responsive: [
          {
            breakpoint: 480,
            options: {
              chart: {
                width: 75,
              },
            },
          },
        ],
        tooltip: {
          custom: ({ series, seriesIndex, dataPointIndex, w }) => {
            console.log('series, seriesIndex, dataPointIndex, w', series, seriesIndex, dataPointIndex, w);
            return '<div class="arrow_box">' + '<span>' + series[seriesIndex][dataPointIndex] + '</span>' + '</div>';
          },
        },
        yaxis: {
          opposite: this.isRtl,
        },
      };
      this.legend = {
        show: false,
        formatter: function (legendName: string, opts?: any) {
          console.log('legendName', legendName);
          console.log('opts', opts);
          // return result[opts.seriesIndex].numberOfTrips + ' ' + legendName;
          return '';
        },
      };
    });
  }
}
