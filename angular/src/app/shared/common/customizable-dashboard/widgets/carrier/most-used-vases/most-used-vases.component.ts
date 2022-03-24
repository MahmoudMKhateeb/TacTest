import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-most-used-vases',
  templateUrl: './most-used-vases.component.html',
  styleUrls: ['./most-used-vases.component.css'],
})
export class MostUsedVasesComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptionsBars>;
  loading = false;

  constructor(private injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._carrierDashboardServiceProxy
      .getMostVasesUsedByShippers()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [
            {
              name: 'Count',
              data: result,
              color: 'rgba(187, 41, 41, 0.847)',
            },
          ],
          chart: {
            type: 'bar',
          },
          plotOptions: {},
          xaxis: {
            type: 'category',
          },
        };
        this.loading = false;
      });
  }
}
