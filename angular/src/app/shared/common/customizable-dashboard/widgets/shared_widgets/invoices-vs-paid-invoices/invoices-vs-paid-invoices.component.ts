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
              data: result.shipperInvoices,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: 'Paid Invoices',
              data: result.paidInvoices,
              color: '#b10303',
            },
          ],
          chart: {
            height: 350,
            type: 'area',
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            curve: 'smooth',
          },
          xaxis: {
            type: 'category',
          },
          tooltip: {
            x: {
              format: 'dd/MM/yy',
            },
            y: {
              formatter: function (val) {
                return val.toFixed(0);
              },
            },
          },
        };
        this.loading = false;
      });
  }
}
