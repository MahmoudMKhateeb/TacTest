import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-deleverd-trips',
  templateUrl: './deleverd-trips.component.html',
  styles: [],
})
export class DeleverdTripsComponent extends AppComponentBase implements OnInit {
  deliveredTripsCount: number;
  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getDeliveredTripsCount().subscribe((result) => {
      this.deliveredTripsCount = result;
    });
  }
}
