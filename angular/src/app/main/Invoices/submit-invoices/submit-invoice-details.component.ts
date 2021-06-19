import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { SubmitInvoiceInfoDto, InvoiceItemDto } from '@shared/service-proxies/service-proxies';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './submit-invoice-details.component.html',
})
export class SubmitInvoiceDetailsComponent extends AppComponentBase {
  Data: SubmitInvoiceInfoDto;
  Items: InvoiceItemDto[];
  loading = false;
  constructor(injector: Injector, private route: ActivatedRoute) {
    super(injector);
    this.Data = this.route.snapshot.data.invoiceinfo;
    this.Items = this.Data.items;
  }
}
