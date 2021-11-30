import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-rated-carriers',
  templateUrl: './top-rated-carriers.component.html',
  styleUrls: ['./top-rated-carriers.component.css'],
})
export class TopRatedCarriersComponent extends AppComponentBase implements OnInit {
  topCarriers: any;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getTopRatedCarriers().subscribe((result) => {
      this.topCarriers = result;
    });
  }
}
