import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-invoice-due-date',
  templateUrl: './invoice-due-date.component.html',
  styleUrls: ['./invoice-due-date.component.css'],
})
export class InvoiceDueDateComponent extends AppComponentBase implements OnInit {
  InvoicesCount: number;
  loading: boolean = false;

  constructor(private injector: Injector, private router: Router, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInvoices();
  }

  getInvoices() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getInvoiceDueDateInDays()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.InvoicesCount = result;
        this.loading = false;
      });
  }
}
