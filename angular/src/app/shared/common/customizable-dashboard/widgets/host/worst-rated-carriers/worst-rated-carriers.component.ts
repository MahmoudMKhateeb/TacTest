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

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getWorstRatedCarriers().subscribe((result) => {
      this.worstRatedCarriers = result;
    });
  }
}
