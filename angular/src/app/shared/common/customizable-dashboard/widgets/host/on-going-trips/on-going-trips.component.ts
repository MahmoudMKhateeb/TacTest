import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-on-going-trips',
  templateUrl: './on-going-trips.component.html',
  styles: [],
})
export class OnGoingTripsComponent implements OnInit {
  tripsCount: number;
  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getOngoingTripsCount().subscribe((result) => {
      this.tripsCount = result;
    });
  }
}
