import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RequestsListPerMonthDto, SalesSummaryDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-invoices-vs-paid-invoices',
  templateUrl: './invoices-vs-paid-invoices.component.html',
  styleUrls: ['./invoices-vs-paid-invoices.component.css'],
})
export class InvoicesVsPaidInvoicesComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  months: string[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  invoices: number[];
  paidInvoices: number[];
  loading: boolean = false;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInvoices();
  }

  getInvoices() {
    this.invoices = [];
    this.paidInvoices = [];
    this.loading = true;

    this._shipperDashboardServiceProxy
      .getInvoicesVSPaidInvoices()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.months.forEach((d) => {
          let i = this.months.indexOf(d) + 1;
          let year = new Date().getFullYear();
          const foundInvoicesElement = result.shipperInvoices.filter((el) => el.month === i);
          if (!foundInvoicesElement) {
            result.shipperInvoices.push(
              new RequestsListPerMonthDto({
                count: 0,
                month: i,
                year: year,
              })
            );
          }
          const foundPaidInvoicesElement = result.paidInvoices.filter((el) => el.month === i);
          if (!foundPaidInvoicesElement) {
            result.paidInvoices.push(
              new RequestsListPerMonthDto({
                count: 0,
                month: i,
                year: year,
              })
            );
          }
        });
        result.shipperInvoices.sort(function (a, b) {
          return a.month - b.month;
        });
        result.shipperInvoices.forEach((element) => {
          this.invoices.push(element.count);
        });
        result.paidInvoices.sort(function (a, b) {
          return a.month - b.month;
        });
        result.paidInvoices.forEach((element) => {
          this.paidInvoices.push(element.count);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Invoices',
              data: this.invoices,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: 'Paid Invoices',
              data: this.paidInvoices,
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
            categories: this.months,
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
