import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-counters-widget',
  templateUrl: './counters-widget.component.html',
  styleUrls: ['./counters-widget.component.css'],
})
export class CountersWidgetComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}
}
