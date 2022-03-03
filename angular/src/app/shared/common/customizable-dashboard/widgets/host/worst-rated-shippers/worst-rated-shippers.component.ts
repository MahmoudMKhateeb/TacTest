import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-worst-rated-shippers',
  templateUrl: './worst-rated-shippers.component.html',
  styleUrls: ['./worst-rated-shippers.component.css'],
})
export class WorstRatedShippersComponent extends AppComponentBase implements OnInit {
  worstRatedShippers: any;
  loading: boolean = false;
  noRequests: number = 0;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getWorstRatedShippers().subscribe((result) => {
      this.noRequests = result.length;
      this.worstRatedShippers = result;
      this.loading = false;
    });
  }
}
