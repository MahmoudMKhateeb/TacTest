import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RequestsListPerMonthDto, SalesSummaryDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-accepted-vs-rejected-pricing',
  templateUrl: './accepted-vs-rejected-pricing.component.html',
  styleUrls: ['./accepted-vs-rejected-pricing.component.css'],
})
export class AcceptedVsRejectedPricingComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  months: string[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  acceptedReqs: number[];
  rejectedReqs: number[];
  fromDate: moment.Moment = null;
  toDate: moment.Moment = null;
  loading: boolean = false;
  appSalesSummaryDateInterval = SalesSummaryDatePeriod;
  selectedDatePeriod: SalesSummaryDatePeriod;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getRequests(this.appSalesSummaryDateInterval.Monthly);
  }

  reload(datePeriod) {
    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getRequests(this.selectedDatePeriod);
  }

  getRequests(datePeriod: SalesSummaryDatePeriod) {
    this.acceptedReqs = [];
    this.rejectedReqs = [];
    this.loading = true;

    this._shipperDashboardServiceProxy
      .getAcceptedAndRejectedRequests(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.months.forEach((d) => {
          let i = this.months.indexOf(d) + 1;
          let year = new Date().getFullYear();
          const foundAcceptElement = result.acceptedRequests.filter((el) => el.month === i);
          if (!foundAcceptElement) {
            result.acceptedRequests.push(
              new RequestsListPerMonthDto({
                count: 0,
                month: i,
                year: year,
                day: foundAcceptElement[i].day,
                week: foundAcceptElement[i].week,
              })
            );
          }
          const foundRejectElement = result.rejectedRequests.filter((el) => el.month === i);
          if (!foundRejectElement) {
            result.rejectedRequests.push(
              new RequestsListPerMonthDto({
                count: 0,
                month: i,
                year: year,
                day: foundAcceptElement[i].day,
                week: foundAcceptElement[i].week,
              })
            );
          }
        });
        result.acceptedRequests.sort(function (a, b) {
          if (datePeriod == SalesSummaryDatePeriod.Monthly) return a.month - b.month;
          if (datePeriod == SalesSummaryDatePeriod.Daily) return a.day - b.day;
          if (datePeriod == SalesSummaryDatePeriod.Weekly) return a.week - b.week;
        });
        result.acceptedRequests.forEach((element) => {
          this.acceptedReqs.push(element.count);
        });
        result.rejectedRequests.sort(function (a, b) {
          if (datePeriod == SalesSummaryDatePeriod.Monthly) return a.month - b.month;
          if (datePeriod == SalesSummaryDatePeriod.Daily) return a.day - b.day;
          if (datePeriod == SalesSummaryDatePeriod.Weekly) return a.week - b.week;
        });
        result.rejectedRequests.forEach((element) => {
          this.rejectedReqs.push(element.count);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Accepted',
              data: this.acceptedReqs,
            },
            {
              name: 'Rejected',
              data: this.rejectedReqs,
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
      });
  }
}
