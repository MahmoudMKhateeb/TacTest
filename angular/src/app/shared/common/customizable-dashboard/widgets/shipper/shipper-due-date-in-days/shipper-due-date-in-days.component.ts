import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-shipper-due-date-in-days',
  templateUrl: './shipper-due-date-in-days.component.html',
  styleUrls: ['./shipper-due-date-in-days.component.css'],
})
export class ShipperDueDateInDaysComponent extends AppComponentBase implements OnInit {
  DocumentsCount: number;
  constructor(private injector: Injector, private router: Router, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._shipperDashboardServiceProxy.getDocumentsDueDateInDays().subscribe((result) => {
      this.DocumentsCount = result;
    });
  }
}
