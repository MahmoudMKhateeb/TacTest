import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-invoices-vs-paid-invoices',
  templateUrl: './invoices-vs-paid-invoices.component.html',
  styleUrls: ['./invoices-vs-paid-invoices.component.css'],
})
export class InvoicesVsPaidInvoicesComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;

  loading = false;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInvoices();
  }

  getInvoices() {
    this.loading = true;

    this._shipperDashboardServiceProxy
      .getInvoicesVSPaidInvoices()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [
            {
              name: 'Invoices',
              data: [6, 8, 25, 15, 10, 18, 22, 23, 25, 30, 38], // result.shipperInvoices,
              // color: 'rgba(187, 41, 41, 0.847)',
              color: '#d7dadc',
            },
            {
              name: 'Paid Invoices',
              color: '#1c1c1c',
              data: [4, 6, 20, 11, 8, 15, 19, 21, 20, 25, 32], //result.paidInvoices,
            },
          ],
          chart: {
            type: 'line',
            width: '100%',
            height: 250,
            zoom: {
              enabled: false,
            },
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            curve: 'smooth',
          },
          grid: {
            row: {
              opacity: 0.5,
            },
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
