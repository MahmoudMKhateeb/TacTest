import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-needs-action-widget',
  templateUrl: './needs-action-widget.component.html',
  styleUrls: ['./needs-action-widget.component.css'],
})
export class NeedsActionWidgetComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}
}
