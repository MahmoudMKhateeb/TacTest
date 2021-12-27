import { Component, Inject, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-drivers-activity',
  templateUrl: './drivers-activity.component.html',
  styleUrls: ['./drivers-activity.component.css'],
})
export class DriversActivityComponent extends AppComponentBase implements OnInit {
  activeDriversCount: number;
  notActiveDriversCount: number;

  constructor(private injector: Injector, private _router: Router, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._carrierDashboardServiceProxy.getDriversActivity().subscribe((result) => {
      this.activeDriversCount = result.activeItems;
      this.notActiveDriversCount = result.notActiveItems;
    });
  }

  getDriversPageByFilter(filter) {
    this._router.navigate(['/app/admin/drivers'], {
      queryParams: {
        isActive: filter,
      },
    });
  }
}
