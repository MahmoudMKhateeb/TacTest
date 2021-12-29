import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-most-used-destinations',
  templateUrl: './most-used-destinations.component.html',
  styleUrls: ['./most-used-destinations.component.css'],
})
export class MostUsedDestinationsComponent extends AppComponentBase implements OnInit {
  data: any;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDestinations();
  }

  getDestinations() {
    this._shipperDashboardServiceProxy.getMostUsedDestinatiions().subscribe((result) => {
      this.data = result;
    });
  }
}
