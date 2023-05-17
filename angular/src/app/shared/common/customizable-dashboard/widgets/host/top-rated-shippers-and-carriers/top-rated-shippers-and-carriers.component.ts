import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-rated-shippers-and-carriers',
  templateUrl: './top-rated-shippers-and-carriers.component.html',
  styleUrls: ['./top-rated-shippers-and-carriers.component.scss'],
})
export class TopRatedShippersAndCarriersComponent extends AppComponentBase implements OnInit {
  constructor(private injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
