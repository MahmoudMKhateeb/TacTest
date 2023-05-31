import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy, TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
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
  selector: 'app-number-of-trips',
  templateUrl: './number-of-trips.component.html',
  styleUrls: ['./number-of-trips.component.scss'],
})
export class NumberOfTripsComponent extends AppComponentBase implements OnInit {
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;

  loading = false;
  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(start: moment.Moment, end: moment.Moment) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getNumberOfTrips(start, end).subscribe((res) => {
      console.log('res', res);
      this.loading = false;
      this.chartOptions = {
        series: [
          {
            name: this.l('Trips'),
            data: res.map((item) => item.y),
            color: '#dc2434',
          },
          // {
          //   name: this.l('Carriers'),
          //   data: [76, 85, 101, 98, 87, 105, 91, 114, 94],
          //   color: '#000',
          // },
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
            // endingShape: 'rounded'
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
          categories: res.map((item) => item.x) /*['Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct']*/,
        },
        yaxis: {
          title: {
            // text: '$ (thousands)'
          },
        },
        fill: {
          opacity: 1,
        },
        tooltip: {
          y: {
            // formatter: function(val) {
            //     return '$ ' + val + ' thousands';
            // }
          },
        },
      };
    });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    console.log('event', event);
    this.getData(event.start, event.end);
  }
}
