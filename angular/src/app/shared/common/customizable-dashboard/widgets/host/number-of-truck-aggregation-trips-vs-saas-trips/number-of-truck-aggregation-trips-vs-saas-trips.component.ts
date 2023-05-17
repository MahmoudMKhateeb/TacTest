import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
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
  selector: 'app-number-of-truck-aggregation-trips-vs-saas-trips',
  templateUrl: './number-of-truck-aggregation-trips-vs-saas-trips.component.html',
  styleUrls: ['./number-of-truck-aggregation-trips-vs-saas-trips.component.scss'],
})
export class NumberOfTruckAggregationTripsVsSaasTripsComponent extends AppComponentBase implements OnInit {
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;

  loading = false;
  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.chartOptions = {
      series: [
        {
          name: this.l('TruckAggregation'),
          data: [44, 55, 57, 56, 61, 58, 63, 60, 66],
          color: '#dc2434',
        },
        {
          name: this.l('SaaSShipment'),
          data: [76, 85, 101, 98, 87, 105, 91, 114, 94],
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
        categories: ['Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'],
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
  }

  getData() {
    this.loading = true;
  }
}
