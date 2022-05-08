import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-on-going-trips',
  templateUrl: './on-going-trips.component.html',
  styles: [],
})
export class OnGoingTripsComponent extends AppComponentBase implements OnInit {
  tripsCount: number;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getOngoingTripsCount().subscribe((result) => {
      this.tripsCount = result;
      this.loading = false;
    });
  }
}
