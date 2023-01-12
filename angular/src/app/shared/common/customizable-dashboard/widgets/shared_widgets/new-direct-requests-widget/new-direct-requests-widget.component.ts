import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarrierDashboardServiceProxy,
  NewDirectRequestListDto,
  NewPriceOfferListDto,
  ShipperDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-new-direct-requests-widget',
  templateUrl: './new-direct-requests-widget.component.html',
  styleUrls: ['./new-direct-requests-widget.component.css'],
})
export class NewDirectRequestsWidgetComponent extends AppComponentBase implements OnInit {
  newDirectRequests: NewDirectRequestListDto[] = [];

  constructor(injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy, private router: Router) {
    super(injector);
  }

  ngOnInit(): void {
    this.getNewDirectRequests();
  }

  getNewDirectRequests(): void {
    this._carrierDashboardServiceProxy.getNewDirectRequest().subscribe((res) => {
      this.newDirectRequests = res;
    });
  }

  goToRequest(item: NewDirectRequestListDto) {
    this.router.navigateByUrl(`/app/main/directrequest/list?srId=${item.shippingRequestId}`);
  }
}
