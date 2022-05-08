import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivityItemsDto, CarrierDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-drivers-activity',
  templateUrl: './drivers-activity.component.html',
  styleUrls: ['./drivers-activity.component.css'],
})
export class DriversActivityComponent extends AppComponentBase implements OnInit {
  items: ActivityItemsDto;

  constructor(private injector: Injector, private _router: Router, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._carrierDashboardServiceProxy.getDriversActivity().subscribe((result) => {
      this.items = result;
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
