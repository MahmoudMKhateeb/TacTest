import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy, MostTenantWorksListDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ChartComponent } from '@node_modules/ng-apexcharts';
import { ApexLegend, ApexPlotOptions } from '@node_modules/ng-apexcharts/lib/model/apex-types';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/shipper/most-worked-with-carriers/most-worked-with-carriers.component';

@Component({
  selector: 'app-most-worked-with-shippers',
  templateUrl: './most-worked-with-shippers.component.html',
  styleUrls: ['./most-worked-with-shippers.component.css'],
})
export class MostWorkedWithShippersComponent extends AppComponentBase implements OnInit {
  Shippers: MostTenantWorksListDto[];
  loading = false;

  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;
  plotOptions: ApexPlotOptions = {
    pie: {
      customScale: 0.7,
      // donut: {
      //     labels: {
      //         name: {
      //             show: true,
      //             formatter: function (val) {
      //                 return val + '%';
      //             }
      //         },
      //         value: {
      //             show: false
      //         },
      //         total: {
      //             show: true
      //         }
      //     }
      // }
    },
  };
  legend: ApexLegend = {};
  constructor(private injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getShippers();
  }

  getShippers() {
    this.loading = true;
    this._carrierDashboardServiceProxy
      .getMostWorkedWithShippers()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.Shippers = result;
        this.chartOptions = {
          series: this.Shippers.map((carrier) => carrier.numberOfTrips) /* [44, 55, 13, 43, 22] */,
          chart: {
            type: 'donut',
          },
          labels: this.Shippers.map((carrier) => carrier.name) /* ["Team A", "Team B", "Team C", "Team D", "Team E"] */,
          responsive: [
            {
              breakpoint: 480,
              options: {
                chart: {
                  width: 200,
                },
                // legend: {
                //   position: 'bottom',
                // },
              },
            },
          ],
        };
        this.legend = {
          formatter: function (legendName: string, opts?: any) {
            console.log('legendName', legendName);
            console.log('opts', opts);
            return result[opts.seriesIndex].numberOfTrips + ' ' + legendName;
          },
        };
        this.loading = false;
      });
  }
}
