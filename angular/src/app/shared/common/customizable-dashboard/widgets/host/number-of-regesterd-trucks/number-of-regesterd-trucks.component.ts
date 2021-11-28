import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-trucks',
  templateUrl: './number-of-regesterd-trucks.component.html',
  styles: [],
})
export class NumberOfRegesterdTrucksComponent implements OnInit {
  trucksCount: number;
  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getTrucksCount().subscribe((result) => {
      this.trucksCount = result;
    });
  }
}
