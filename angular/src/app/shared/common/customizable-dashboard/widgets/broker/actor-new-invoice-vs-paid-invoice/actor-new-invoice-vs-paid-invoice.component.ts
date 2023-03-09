import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-actor-new-invoice-vs-paid-invoice',
  templateUrl: './actor-new-invoice-vs-paid-invoice.component.html',
  styleUrls: ['./actor-new-invoice-vs-paid-invoice.component.scss'],
})
export class ActorNewInvoiceVsPaidInvoiceComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
