import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

@Component({
  selector: 'app-unpriced-requests-in-marketplace',
  templateUrl: './unpriced-requests-in-marketplace.component.html',
  styleUrls: ['./unpriced-requests-in-marketplace.component.css'],
})
export class UnpricedRequestsInMarketplaceComponent extends AppComponentBase implements OnInit {
  requests: any;
  endDate: moment.Moment = null;
  startDate: moment.Moment = null;
  loading: boolean = false;
  saving = false;
  noRequests: number = 0;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getUnpricedRequests();
  }

  getUnpricedRequests() {
    this.saving = true;
    this.loading = true;
    this._hostDashboardServiceProxy.getUnpricedRequestsInMarketplace(this.startDate, this.endDate).subscribe((result) => {
      this.requests = result;
      console.log('req', this.requests);
      this.noRequests = this.requests.length;
      this.loading = false;
      this.saving = false;
    });
  }
}
