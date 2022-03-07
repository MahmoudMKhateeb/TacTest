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
  toDate: moment.Moment = null;
  fromDate: moment.Moment = null;
  loading: boolean = false;
  saving = false;
  appSalesSummaryDateInterval = SalesSummaryDatePeriod;
  selectedDatePeriod: SalesSummaryDatePeriod;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInvoices(this.appSalesSummaryDateInterval.Daily);
  }

  reload(datePeriod) {
    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getInvoices(this.selectedDatePeriod);
  }

  getInvoices(datePeriod: SalesSummaryDatePeriod) {
    this.invoices = [];
    this.paidInvoices = [];
    this.loading = true;
    this.saving = true;

    this._shipperDashboardServiceProxy
      .getInvoicesVSPaidInvoices(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.saving = false;
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
                day: foundInvoicesElement[i].day,
                week: foundInvoicesElement[i].week,
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
                day: foundPaidInvoicesElement[i].day,
                week: foundPaidInvoicesElement[i].week,
              })
            );
          }
        });
        result.shipperInvoices.sort(function (a, b) {
          if (datePeriod == SalesSummaryDatePeriod.Monthly) return a.month - b.month;
          if (datePeriod == SalesSummaryDatePeriod.Daily) return a.day - b.day;
          if (datePeriod == SalesSummaryDatePeriod.Weekly) return a.week - b.week;
        });
        result.shipperInvoices.forEach((element) => {
          this.invoices.push(element.count);
        });
        result.paidInvoices.sort(function (a, b) {
          if (datePeriod == SalesSummaryDatePeriod.Monthly) return a.month - b.month;
          if (datePeriod == SalesSummaryDatePeriod.Daily) return a.day - b.day;
          if (datePeriod == SalesSummaryDatePeriod.Weekly) return a.week - b.week;
        });
        result.paidInvoices.forEach((element) => {
          this.paidInvoices.push(element.count);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Invoices',
              data: this.invoices,
            },
            {
              name: 'Paid Invoices',
              data: this.paidInvoices,
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
          },
        };
        this.loading = false;
        this.saving = false;
      });
  }
}
