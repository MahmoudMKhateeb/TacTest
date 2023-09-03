import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import {
  ApexAxisChartSeries,
  ApexChart,
  ChartComponent,
  ApexDataLabels,
  ApexPlotOptions,
  ApexYAxis,
  ApexLegend,
  ApexStroke,
  ApexXAxis,
  ApexFill,
  ApexTooltip,
} from 'ng-apexcharts';
import * as moment from '@node_modules/moment';

export interface ChartOptions {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
}

@Component({
  selector: 'app-normal-requests-vs-dedicated-requests',
  templateUrl: './normal-requests-vs-dedicated-requests.component.html',
  styleUrls: ['./normal-requests-vs-dedicated-requests.component.scss'],
})
export class NormalRequestsVSDedicatedRequestsComponent extends AppComponentBase implements OnInit {
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;

  loading = false;
  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(start: moment.Moment, end: moment.Moment) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getNormalVsDedicatedRequests(start, end).subscribe((res) => {
      this.loading = false;
      this.chartOptions = {
        series: [
          {
            name: this.l('Normal'),
            data: res.normalTrips.map((item) => item.y),
            color: '#dc2434',
          },
          {
            name: this.l('Dedicated'),
            data: res.dedicatedTrips.map((item) => item.y),
            color: '#000',
          },
        ],
        chart: {
          type: 'bar',
          height: '100%',
          width: '100%',
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
          categories: res.normalTrips.map((item) => item.x),
        },
        yaxis: {
          title: {},
        },
        fill: {
          opacity: 1,
        },
        tooltip: {
          y: {},
        },
      };
    });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    this.getData(event.start, event.end);
  }
}
