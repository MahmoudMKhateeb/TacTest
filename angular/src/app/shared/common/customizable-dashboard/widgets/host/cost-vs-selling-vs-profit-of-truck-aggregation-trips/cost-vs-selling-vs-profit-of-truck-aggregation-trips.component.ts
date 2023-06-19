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
  selector: 'app-cost-vs-selling-vs-profit-of-truck-aggregation-trips',
  templateUrl: './cost-vs-selling-vs-profit-of-truck-aggregation-trips.component.html',
  styleUrls: ['./cost-vs-selling-vs-profit-of-truck-aggregation-trips.component.scss'],
})
export class CostVsSellingVsProfitOfTruckAggregationTripsComponent extends AppComponentBase implements OnInit {
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
      .getCostVsSellingTruckAggregationTrips(event.start, event.end)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.chartOptions = {
          series: [
            {
              name: this.l('Cost'),
              data: res.cost.map((item) => item.y),
              color: '#dc2434',
            },
            {
              name: this.l('Selling'),
              data: res.selling.map((item) => item.y),
              color: '#000',
            },
            {
              name: this.l('Profit'),
              data: res.profit.map((item) => item.y),
              color: '#707070',
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
            categories: res.cost.map((item) => item.x),
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
    this.getData(event);
  }
}
