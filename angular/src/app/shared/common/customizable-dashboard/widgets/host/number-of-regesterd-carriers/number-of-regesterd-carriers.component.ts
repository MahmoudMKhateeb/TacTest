import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-carriers',
  templateUrl: './number-of-regesterd-carriers.component.html',
  styles: [],
})
export class NumberOfRegesterdCarriersComponent implements OnInit {
  carriersCount: number;
  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getCarriersCount().subscribe((result) => {
      this.carriersCount = result;
    });
  }
}
