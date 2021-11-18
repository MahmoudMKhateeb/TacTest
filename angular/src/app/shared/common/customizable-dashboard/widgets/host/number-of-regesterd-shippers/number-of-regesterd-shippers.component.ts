import { Component, OnInit } from '@angular/core';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-shippers',
  templateUrl: './number-of-regesterd-shippers.component.html',
  styles: [],
})
export class NumberOfRegesterdShippersComponent implements OnInit {
  shippersCount: number;
  constructor(private _hostDashboardServiceProxy: HostDashboardServiceProxy) {}

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getShippersCount().subscribe((result) => {
      this.shippersCount = result;
    });
  }
}
