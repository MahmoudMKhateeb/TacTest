import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-actor-new-invoice-vs-paid-invoice',
  templateUrl: './actor-paid-invoice-vs-claimed-invoice.component.html',
  styleUrls: ['./actor-paid-invoice-vs-claimed-invoice.component.scss'],
})
export class ActorPaidInvoiceVsClaimedInvoiceComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
