import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { CarrierDashboardServiceProxy, ChartCategoryPairedValuesDto } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-carrier-invoices-details-widget',
  templateUrl: './carrier-invoices-details-widget.component.html',
  styleUrls: ['./carrier-invoices-details-widget.component.css'],
})
export class CarrierInvoicesDetailsWidgetComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptionsBars>;

  loading = false;
  acceptedVsRejected: { total: number; paid: number; unpaid: number };

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
        const paid = result.paidInvoices.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
        const unpaidResult = result.claimed.concat(result.newInvoices);
        const unpaid = unpaidResult.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
        this.acceptedVsRejected = {
          paid,
          unpaid,
          total: paid + unpaid,
        };
        const categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        const paidSeries = categories.map((item) => {
          const foundFromResponse = result.paidInvoices.find((accepted) => {
            accepted.x = accepted?.x.slice(0, 3);
            return accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
          console.log('acceptedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        const unpaidSeries = categories.map((item) => {
          const foundFromResponse = unpaidResult.find((rejected) => {
            rejected.x = rejected?.x.slice(0, 3);
            return rejected.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
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
          xaxis: {
            type: 'category',
            categories,
          },
          yaxis: {
            min: 0,
            tickAmount: 1,
            floating: false,
            decimalsInFloat: 0,
          },
        };
        (this.chartOptions as any).grid = {
          row: {
            opacity: 0.5,
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
