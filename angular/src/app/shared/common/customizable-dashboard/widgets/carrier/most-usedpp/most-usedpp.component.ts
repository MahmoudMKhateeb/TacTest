import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-most-usedpp',
  templateUrl: './most-usedpp.component.html',
  styleUrls: ['./most-usedpp.component.css'],
})
export class MostUsedppComponent extends AppComponentBase implements OnInit {
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
      .getMostPricePackageByShippers()
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
            width: 450,
            height: 200,
          },
          plotOptions: {
            bar: {
              horizontal: false,
              columnWidth: '55%',
            },
          },
          xaxis: {
            type: 'category',
            axisTicks: {
              show: false,
            },
          },
          stroke: {
            show: true,
            width: 2,
            colors: ['transparent'],
          },
          yaxis: {
            labels: {
              show: false,
            },
          },
          tooltip: {
            enabled: false,
          },
          fill: {
            opacity: 1,
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
        this.loading = false;
      });
  }
}
