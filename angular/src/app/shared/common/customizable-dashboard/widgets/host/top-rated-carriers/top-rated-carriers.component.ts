import { Component, HostListener, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetTenantsCountWithRateOutput, HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-rated-carriers',
  templateUrl: './top-rated-carriers.component.html',
  styleUrls: ['./top-rated-carriers.component.css'],
})
export class TopRatedCarriersComponent extends AppComponentBase implements OnInit {
  @Input() topCarriers: GetTenantsCountWithRateOutput[] = [];

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}
}
