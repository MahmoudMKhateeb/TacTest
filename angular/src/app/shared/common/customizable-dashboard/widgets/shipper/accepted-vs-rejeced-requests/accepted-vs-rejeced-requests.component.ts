import { Component, Injectable, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ListPerMonthDto,
  RequestsListPerMonthDto,
  SalesSummaryDatePeriod,
  ShipperDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-accepted-vs-rejeced-requests',
  templateUrl: './accepted-vs-rejeced-requests.component.html',
  styleUrls: ['./accepted-vs-rejeced-requests.component.css'],
})
export class AcceptedVsRejecedRequestsComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  months: string[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  acceptedReqs: number[];
  rejectedReqs: number[];
  toDate: moment.Moment = null;
  fromDate: moment.Moment = null;
  loading: boolean = false;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    this.acceptedReqs = [];
    this.rejectedReqs = [];
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getAcceptedAndRejectedRequests()
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
              })
            );
          }
        });
        result.acceptedRequests.sort(function (a, b) {
          return a.month - b.month;
        });
        result.acceptedRequests.forEach((element) => {
          this.acceptedReqs.push(element.count);
        });
        result.rejectedRequests.sort(function (a, b) {
          return a.month - b.month;
        });
        result.rejectedRequests.forEach((element) => {
          this.rejectedReqs.push(element.count);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Accepted',
              data: this.acceptedReqs,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: 'Rejected',
              data: this.rejectedReqs,
              color: '#b5b5c3',
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
