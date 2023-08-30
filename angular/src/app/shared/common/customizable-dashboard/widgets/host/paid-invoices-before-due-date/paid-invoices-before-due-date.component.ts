import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetInvoicesPaidBeforeDueDateOutput, TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-paid-invoices-before-due-date',
  templateUrl: './paid-invoices-before-due-date.component.html',
  styleUrls: ['./paid-invoices-before-due-date.component.scss'],
})
export class PaidInvoicesBeforeDueDateComponent extends AppComponentBase implements OnInit {
  loading = false;
  paidInvoiceBeforeDueDate: GetInvoicesPaidBeforeDueDateOutput;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(event: { start: moment.Moment; end: moment.Moment }) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getInvoicesPaidBeforeDueDate()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.paidInvoiceBeforeDueDate = res;
      });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    this.getData(event);
  }
}
