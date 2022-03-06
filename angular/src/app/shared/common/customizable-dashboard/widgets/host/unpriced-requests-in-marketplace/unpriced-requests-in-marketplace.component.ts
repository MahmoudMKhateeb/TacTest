import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy, SalesSummaryDatePeriod } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-unpriced-requests-in-marketplace',
  templateUrl: './unpriced-requests-in-marketplace.component.html',
  styleUrls: ['./unpriced-requests-in-marketplace.component.css'],
})
export class UnpricedRequestsInMarketplaceComponent extends AppComponentBase implements OnInit {
  requests: any;
  toDate: moment.Moment = null;
  fromDate: moment.Moment = null;
  loading: boolean = false;
  saving = false;
  noRequests: number = 0;
  appSalesSummaryDateInterval = SalesSummaryDatePeriod;
  selectedDatePeriod: SalesSummaryDatePeriod;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getUnpricedRequests(this.appSalesSummaryDateInterval.Daily);
  }

  reload(datePeriod) {
    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getUnpricedRequests(this.selectedDatePeriod);
  }

  getUnpricedRequests(datePeriod: SalesSummaryDatePeriod) {
    this.saving = true;
    this.loading = true;
    this._hostDashboardServiceProxy
      .getUnpricedRequestsInMarketplace(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.requests = result;
        this.noRequests = this.requests.length;
        this.loading = false;
        this.saving = false;
      });
  }
}
