import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-carrier-invoices-details-widget',
  templateUrl: './carrier-invoices-details-widget.component.html',
  styleUrls: ['./carrier-invoices-details-widget.component.css'],
})
export class CarrierInvoicesDetailsWidgetComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;

  loading = false;

  constructor(injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInvoices();
  }

  getInvoices() {
    this.loading = true;

    this._carrierDashboardServiceProxy
      .getCarrierInvoicesDetails()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [
            {
              name: this.l('Total'),
              data: result.total,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: this.l('Paid'),
              data: result.paidInvoices,
            },
            {
              name: this.l('New'),
              data: result.newInvoices,
            },
            {
              name: this.l('Claimed'),
              data: result.claimed,
            },
          ],
          chart: {
            type: 'bar',
            width: 400,
            height: 210,
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
        this.loading = false;
      });
  }
}
