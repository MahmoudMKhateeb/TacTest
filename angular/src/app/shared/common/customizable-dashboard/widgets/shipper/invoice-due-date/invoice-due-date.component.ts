import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-invoice-due-date',
  templateUrl: './invoice-due-date.component.html',
  styleUrls: ['./invoice-due-date.component.css'],
})
export class InvoiceDueDateComponent extends AppComponentBase implements OnInit {
  InvoicesCount: number;
  constructor(private injector: Injector, private router: Router, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._shipperDashboardServiceProxy.getInvoiceDueDateInDays().subscribe((result) => {
      this.InvoicesCount = result;
    });
  }
}
