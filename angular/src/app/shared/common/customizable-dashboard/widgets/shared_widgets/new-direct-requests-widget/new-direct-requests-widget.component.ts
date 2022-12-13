import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-new-direct-requests-widget',
  templateUrl: './new-direct-requests-widget.component.html',
  styleUrls: ['./new-direct-requests-widget.component.css'],
})
export class NewDirectRequestsWidgetComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}
}
