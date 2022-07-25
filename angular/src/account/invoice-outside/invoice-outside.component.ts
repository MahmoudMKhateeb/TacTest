import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { InvoiceOutSideDto, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-invoice-outside',
  templateUrl: './invoice-outside.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class InvoiceOutsideComponent extends AppComponentBase implements OnInit {
  invoice: InvoiceOutSideDto;
  constructor(injector: Injector, private _invoiceAppService: InvoiceServiceProxy, private _activatedRoute: ActivatedRoute) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInvoice();
  }
  getInvoice(): void {
    this._invoiceAppService.getInvoiceOutSide(this._activatedRoute.snapshot.queryParams['id']).subscribe((res) => {
      this.invoice = res;
    });
  }
}
