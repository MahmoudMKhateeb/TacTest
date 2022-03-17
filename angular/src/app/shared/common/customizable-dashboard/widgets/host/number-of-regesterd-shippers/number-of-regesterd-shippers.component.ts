import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-shippers',
  templateUrl: './number-of-regesterd-shippers.component.html',
  styles: [],
})
export class NumberOfRegesterdShippersComponent extends AppComponentBase implements OnInit {
  shippersCount: number;
  loading: boolean = false;
  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getShippersCount().subscribe((result) => {
      this.shippersCount = result;
      this.loading = false;
    });
  }
}
