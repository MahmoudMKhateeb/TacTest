import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-rated-shippers',
  templateUrl: './top-rated-shippers.component.html',
  styleUrls: ['./top-rated-shippers.component.css'],
})
export class TopRatedShippersComponent extends AppComponentBase implements OnInit {
  topShippers: any;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._hostDashboardServiceProxy.getTopRatedShippers().subscribe((result) => {
      this.topShippers = result;
    });
  }
}
