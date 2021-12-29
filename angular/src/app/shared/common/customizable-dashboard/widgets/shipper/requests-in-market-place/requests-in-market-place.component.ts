import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

@Component({
  selector: 'app-requests-in-market-place',
  templateUrl: './requests-in-market-place.component.html',
  styleUrls: ['./requests-in-market-place.component.css'],
})
export class RequestsInMarketPlaceComponent extends AppComponentBase implements OnInit {
  Requests: any;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getRequests();
  }

  getRequests() {
    this._shipperDashboardServiceProxy.getRequestsInMarketpalce().subscribe((result) => {
      this.Requests = result;
    });
  }
}
