import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-worst-rated-carriers',
  templateUrl: './worst-rated-carriers.component.html',
  styleUrls: ['./worst-rated-carriers.component.css'],
})
export class WorstRatedCarriersComponent extends AppComponentBase implements OnInit {
  worstRatedCarriers: any;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._hostDashboardServiceProxy.getWorstRatedCarriers().subscribe((result) => {
      this.worstRatedCarriers = result;
      this.loading = false;
    });
  }
}
