import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-overall-total-amount-per-all-trips',
  templateUrl: './overall-total-amount-per-all-trips.component.html',
  styleUrls: ['./overall-total-amount-per-all-trips.component.scss'],
})
export class OverallTotalAmountPerAllTripsComponent extends AppComponentBase implements OnInit {
  loading = false;

  selling = 20000;
  cost = 15000;
  profit = 5000;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    // this.loading = true;
  }
}
