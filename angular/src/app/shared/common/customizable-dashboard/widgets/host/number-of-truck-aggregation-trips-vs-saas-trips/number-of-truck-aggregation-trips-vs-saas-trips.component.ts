import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
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
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

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
  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(event: { start: moment.Moment; end: moment.Moment }) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getNumberOfTruckAggregVsSAASTrips(event.start, event.end)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.chartOptions = {
          series: [
            {
              name: this.l('TruckAggregation'),
              data: res.truckAggregationTrips.map((item) => item.y),
              color: '#dc2434',
            },
            {
              name: this.l('SaaSShipment'),
              data: res.saasTrips.map((item) => item.y),
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
            categories: res.truckAggregationTrips.map((item) => item.x),
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
        this.loading = false;
      });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    this.getData(event);
  }
}
