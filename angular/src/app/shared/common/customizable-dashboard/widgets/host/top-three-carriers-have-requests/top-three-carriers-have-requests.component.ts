import { Component, Injector, OnInit } from '@angular/core';
import { inject } from '@angular/core/testing';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-three-carriers-have-requests',
  templateUrl: './top-three-carriers-have-requests.component.html',
  styleUrls: ['./top-three-carriers-have-requests.component.css'],
})
export class TopThreeCarriersHaveRequestsComponent extends AppComponentBase implements OnInit {
  Carriers: any;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getCarriersHaveMostRequests().subscribe((result) => {
      this.Carriers = result;
      this.loading = false;
    });
  }
}
