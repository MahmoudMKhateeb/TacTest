import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FilterDatePeriod, SalesSummaryDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';
import { WidgetComponentBase } from '../../widget-component-base';

@Component({
  selector: 'app-requests-in-market-place',
  templateUrl: './requests-in-market-place.component.html',
  styleUrls: ['./requests-in-market-place.component.css'],
})
export class RequestsInMarketPlaceComponent extends WidgetComponentBase implements OnInit {
  Requests: any;
  loading: boolean = false;
  saving = false;
  filterDatePeriodInterval = FilterDatePeriod;
  selectedDatePeriod: FilterDatePeriod;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.runDelayed(() => {
      this.getRequests(this.filterDatePeriodInterval.Daily);
    });
  }

  reload(datePeriod) {
    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getRequests(this.selectedDatePeriod);
  }

  getRequests(datePeriod: FilterDatePeriod) {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getRequestsInMarketpalce(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.Requests = result;
        this.loading = false;
      });
  }
}
