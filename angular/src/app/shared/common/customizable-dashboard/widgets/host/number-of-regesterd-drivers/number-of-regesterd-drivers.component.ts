import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-drivers',
  templateUrl: './number-of-regesterd-drivers.component.html',
  styles: [],
})
export class NumberOfRegesterdDriversComponent extends AppComponentBase implements OnInit {
  driversCount: number;
  loading: boolean = false;
  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getDriversCount().subscribe((result) => {
      this.driversCount = result;
      this.loading = false;
    });
  }
}
