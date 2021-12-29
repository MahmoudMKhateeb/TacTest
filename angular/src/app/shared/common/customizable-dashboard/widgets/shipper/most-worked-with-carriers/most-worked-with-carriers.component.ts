import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-most-worked-with-carriers',
  templateUrl: './most-worked-with-carriers.component.html',
  styleUrls: ['./most-worked-with-carriers.component.css'],
})
export class MostWorkedWithCarriersComponent extends AppComponentBase implements OnInit {
  Carriers: any;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._shipperDashboardServiceProxy.getMostWorkedWithCarriers().subscribe((result) => {
      this.Carriers = result;
    });
  }
}
