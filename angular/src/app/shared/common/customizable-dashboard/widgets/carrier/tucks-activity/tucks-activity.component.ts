import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-tucks-activity',
  templateUrl: './tucks-activity.component.html',
  styleUrls: ['./tucks-activity.component.css'],
})
export class TucksActivityComponent extends AppComponentBase implements OnInit {
  activeTrucksCount: number;
  notActiveTrucksCount: number;

  constructor(private injector: Injector, private _router: Router, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._carrierDashboardServiceProxy.getTrucksActivity().subscribe((result) => {
      this.activeTrucksCount = result.activeItems;
      this.notActiveTrucksCount = result.notActiveItems;
    });
  }

  getTrucksPageByFilter(filter) {
    this._router.navigate(['/app/main/trucks/trucks'], {
      queryParams: {
        Active: filter,
      },
    });
  }
}
