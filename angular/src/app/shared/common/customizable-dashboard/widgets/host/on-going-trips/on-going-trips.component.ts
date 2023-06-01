import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-on-going-trips',
  templateUrl: './on-going-trips.component.html',
  styleUrls: ['./on-going-trips.component.scss'],
})
export class OnGoingTripsComponent extends AppComponentBase implements OnInit {
  tripsCount: number;
  loading = false;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getInTransitTripsInCurrentMonth().subscribe((result) => {
      this.tripsCount = result;
      this.loading = false;
    });
  }
}
