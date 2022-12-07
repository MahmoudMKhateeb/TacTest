import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-new-offers-widget',
  templateUrl: './new-offers-widget.component.html',
  styleUrls: ['./new-offers-widget.component.css'],
})
export class NewOffersWidgetComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}
}
