import { Component, Injector, Input, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DedicatedShippingRequestsServiceProxy, ShipperDashboardServiceProxy, UpdateRequestKPIInput } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-project-performance',
  templateUrl: './project-performance.component.html',
  styleUrls: ['./project-performance.component.css'],
})
export class ProjectPerformanceComponent extends AppComponentBase implements OnInit {
  @Input('shippingRequestId') shippingRequestId: number;
  @Input('KPI') kpi: number;
  @Input('numberOfTrips') numberOfTrips: number;
  public chartOptions: Partial<ChartOptions>;
  loading = false;
  showModifyKpi: boolean;

  constructor(injector: Injector, private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.fillChartData();
  }

  fillChartData() {
    // this.loading = true;
    //
    // this._shipperDashboardServiceProxy
    //   .getCompletedTripVsPod()
    //   .pipe(
    //     finalize(() => {
    //       this.loading = false;
    //     })
    //   )
    //   .subscribe((result) => {
    this.chartOptions = {
      series: [
        {
          name: this.l('KPI'),
          data: [{ y: this.kpi, x: '' }],
          color: 'rgba(154,154,154,0.84)',
        },
        {
          name: this.l('NumberOfTrips'),
          data: [{ y: this.numberOfTrips, x: '' }],
          color: 'rgba(187, 41, 41, 0.847)',
        },
      ],
      chart: {
        type: 'bar',
        width: 400,
        height: 250,
      },
      xaxis: {
        type: 'category',
      },
    };
    (this.chartOptions.chart.locales as any[]) = [
      {
        name: 'en',
        options: {
          toolbar: {
            exportToPNG: this.l('Download') + ' PNG',
            exportToSVG: this.l('Download') + ' SVG',
            exportToCSV: this.l('Download') + ' CSV',
          },
        },
      },
    ];
    //     this.loading = false;
    //   });
  }

  updateKPI() {
    const input = new UpdateRequestKPIInput({
      shippingRequestId: this.shippingRequestId,
      kpi: this.kpi,
    });
    this.loading = true;
    this._dedicatedShippingRequestsServiceProxy.updateRequestKPI(input).subscribe((res) => {
      this.notify.success(this.l('UpdatedSuccessfully'));
      this.loading = false;
      this.showModifyKpi = false;
    });
  }
}
