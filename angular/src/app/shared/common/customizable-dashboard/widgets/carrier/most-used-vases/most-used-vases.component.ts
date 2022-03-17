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
  types: string[];
  counts: number[];
  loading: boolean = false;

  constructor(private injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.types = [];
    this.counts = [];
    this.loading = true;
    this._carrierDashboardServiceProxy
      .getMostVasesUsedByShippers()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          this.types.push(element.vasType);
          this.counts.push(element.availableVasTypeCount);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Count',
              data: this.counts,
              color: 'rgba(187, 41, 41, 0.847)',
            },
          ],
          chart: {
            type: 'bar',
            height: 350,
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
            categories: this.types,
          },
          tooltip: {
            y: {
              formatter: function (val) {
                return val.toFixed(0);
              },
            },
          },
          fill: {
            opacity: 1,
          },
        };
        this.loading = false;
      });
  }
}
