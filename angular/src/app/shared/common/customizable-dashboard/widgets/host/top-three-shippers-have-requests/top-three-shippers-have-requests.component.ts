import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-three-shippers-have-requests',
  templateUrl: './top-three-shippers-have-requests.component.html',
  styleUrls: ['./top-three-shippers-have-requests.component.css'],
})
export class TopThreeShippersHaveRequestsComponent extends AppComponentBase implements OnInit {
  Shippers: any;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getShippersHaveMostRequests().subscribe((result) => {
      this.Shippers = result;
      this.loading = false;
    });
  }
}
