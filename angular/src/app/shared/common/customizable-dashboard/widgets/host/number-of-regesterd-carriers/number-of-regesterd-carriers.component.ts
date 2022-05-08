import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-carriers',
  templateUrl: './number-of-regesterd-carriers.component.html',
  styles: [],
})
export class NumberOfRegesterdCarriersComponent extends AppComponentBase implements OnInit {
  carriersCount: number;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getCarriersCount().subscribe((result) => {
      this.carriersCount = result;
      this.loading = false;
    });
  }
}
