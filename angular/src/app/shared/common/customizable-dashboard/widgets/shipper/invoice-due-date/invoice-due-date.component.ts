import { Component, Injector, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BrokerInvoiceType, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-invoice-due-date',
  templateUrl: './invoice-due-date.component.html',
  styleUrls: ['./invoice-due-date.component.css'],
})
export class InvoiceDueDateComponent extends AppComponentBase implements OnInit {
  private _invoiceType: BrokerInvoiceType;
  @Input('invoiceType')
  set invoiceType(val: BrokerInvoiceType) {
    this._invoiceType = val;
    this.getInvoices();
  }
  get invoiceType(): BrokerInvoiceType {
    return this._invoiceType;
  }
  InvoicesCount: number;
  InvoiceTimeUnit: string;
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
      .getInvoiceDueDateInDays(this.invoiceType)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.InvoicesCount = result.count;
        this.InvoiceTimeUnit = result.timeUnit;
        this.loading = false;
      });
  }
}
