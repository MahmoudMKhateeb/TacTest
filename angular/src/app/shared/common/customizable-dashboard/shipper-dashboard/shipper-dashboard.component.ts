import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-shipper-dashboard',
  templateUrl: './shipper-dashboard.component.html',
  styleUrls: ['./shipper-dashboard.component.css'],
})
export class ShipperDashboardComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
