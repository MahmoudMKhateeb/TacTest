import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-deleverd-trips',
  templateUrl: './deleverd-trips.component.html',
  styles: [],
})
export class DeleverdTripsComponent implements OnInit {
  deliveredTripsCount: number;
  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getDeliveredTripsCount().subscribe((result) => {
      this.deliveredTripsCount = result;
    });
  }
}
