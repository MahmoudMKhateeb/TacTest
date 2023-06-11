import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MostTenantWorksListDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ChartComponent } from 'ng-apexcharts';
import { ApexNonAxisChartSeries, ApexResponsive, ApexChart } from 'ng-apexcharts';
import { ApexLegend, ApexPlotOptions } from '@node_modules/ng-apexcharts/lib/model/apex-types';

export interface ChartOptions {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
}

@Component({
  selector: 'app-most-worked-with-carriers',
  templateUrl: './most-worked-with-carriers.component.html',
  styleUrls: ['./most-worked-with-carriers.component.css'],
})
export class MostWorkedWithCarriersComponent extends AppComponentBase implements OnInit {
  Carriers: MostTenantWorksListDto[] = [];
  loading = false;
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;
  plotOptions: ApexPlotOptions = {
    pie: {
      customScale: 1,
    },
  };
  legend: ApexLegend = {};

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getCarriers();
  }

  getCarriers() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getMostWorkedWithCarriers()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.Carriers = result;
        this.chartOptions = {
          series: this.Carriers.map((carrier) => carrier.numberOfTrips),
          chart: {
            type: 'donut',
            width: '100%',
            height: 250,
          },
          labels: this.Carriers.map((carrier) => carrier.name),
          responsive: [
            {
              breakpoint: 480,
              options: {
                chart: {
                  width: 200,
                },
              },
            },
          ],
        };
        this.legend = {
          formatter: function (legendName: string, opts?: any) {
            return result[opts.seriesIndex].numberOfTrips + ' ' + legendName;
          },
        };
        this.loading = false;
      });
  }
}
