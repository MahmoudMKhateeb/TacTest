import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-regesterd-trucks',
  templateUrl: './number-of-regesterd-trucks.component.html',
  styles: [],
})
export class NumberOfRegesterdTrucksComponent extends AppComponentBase implements OnInit {
  trucksCount: number;
  loading: boolean = false;
  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getTrucksCount().subscribe((result) => {
      this.trucksCount = result;
      this.loading = false;
    });
  }
}
