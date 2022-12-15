import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartCategoryPairedValuesDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-invoices-vs-paid-invoices',
  templateUrl: './invoices-vs-paid-invoices.component.html',
  styleUrls: ['./invoices-vs-paid-invoices.component.css'],
})
export class InvoicesVsPaidInvoicesComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;

  loading = false;
  acceptedVsRejected: { total: number; paid: number; unpaid: number };

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
        const paid = result.paidInvoices.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
        const unpaid = result.shipperInvoices.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
        this.acceptedVsRejected = {
          paid,
          unpaid,
          total: paid + unpaid,
        };
        const categories = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        const paidSeries = categories.map((item) => {
          const foundFromResponse = result.paidInvoices.find((accepted) => accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase());
          console.log('acceptedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        const unpaidSeries = categories.map((item) => {
          const foundFromResponse = result.shipperInvoices.find((rejected) => rejected.x.toLocaleLowerCase() === item.toLocaleLowerCase());
          console.log('rejectedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        console.log('paidSeries', paidSeries);
        console.log('unpaidSeries', unpaidSeries);
        this.chartOptions = {
          series: [
            {
              name: this.isShipper ? this.l('UnPaidInvoice') : this.l('Claimed'),
              // data: [6, 8, 25, 15, 10, 18, 22, 23, 25, 30, 38], // result.shipperInvoices,
              data: unpaidSeries,
              // color: 'rgba(187, 41, 41, 0.847)',
              color: '#d7dadc',
            },
            {
              name: this.isShipper ? this.l('PaidInvoice') : this.l('Paid'),
              color: '#dc2434',
              // data: [4, 6, 20, 11, 8, 15, 19, 21, 20, 25, 32], //result.paidInvoices,
              data: paidSeries,
            },
          ],
          chart: {
            type: 'line',
            width: '100%',
            height: 350,
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
            categories,
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
