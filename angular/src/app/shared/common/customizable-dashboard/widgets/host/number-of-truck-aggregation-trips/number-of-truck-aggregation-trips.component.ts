import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy, TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-truck-aggregation-trips',
  templateUrl: './number-of-truck-aggregation-trips.component.html',
  styleUrls: ['./number-of-truck-aggregation-trips.component.scss'],
})
export class NumberOfTruckAggregationTripsComponent extends AppComponentBase implements OnInit {
  tripsCount: number;
  loading: boolean = false;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getTruckAggregationTripsNumber().subscribe((result) => {
      this.tripsCount = result;
      this.loading = false;
    });
  }
}
