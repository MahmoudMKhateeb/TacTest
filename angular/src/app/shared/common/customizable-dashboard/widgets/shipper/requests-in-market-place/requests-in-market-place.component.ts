import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-requests-in-market-place',
  templateUrl: './requests-in-market-place.component.html',
  styleUrls: ['./requests-in-market-place.component.css'],
})
export class RequestsInMarketPlaceComponent extends AppComponentBase implements OnInit {
  Requests: any;
  toDate: moment.Moment = null;
  fromDate: moment.Moment = null;
  loading: boolean = false;
  saving = false;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getRequests();
  }

  getRequests() {
    this.loading = true;
    this.saving = true;
    this._shipperDashboardServiceProxy
      .getRequestsInMarketpalce(this.fromDate, this.toDate)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.saving = false;
        })
      )
      .subscribe((result) => {
        this.Requests = result;
        this.loading = false;
        this.saving = false;
      });
  }
}
