import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-drivers',
  templateUrl: './number-of-regesterd-drivers.component.html',
  styles: [],
})
export class NumberOfRegesterdDriversComponent implements OnInit {
  driversCount: number;
  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getDriversCount().subscribe((result) => {
      this.driversCount = result;
    });
  }
}
