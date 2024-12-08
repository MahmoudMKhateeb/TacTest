import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { SubmitInvoiceInfoDto, InvoiceItemDto } from '@shared/service-proxies/service-proxies';

@Component({
  animations: [appModuleAnimation()],
  styleUrls: ['../template/invoice-template.component.scss'],
  templateUrl: './invoice-tenant-details.component.html',
})
export class InvoiceTenantDetailsComponent extends AppComponentBase {
  Data: SubmitInvoiceInfoDto;
  Items: InvoiceItemDto[];
  loading = false;
  constructor(injector: Injector, private route: ActivatedRoute) {
    super(injector);
    this.Data = this.route.snapshot.data.invoiceinfo;
    this.Items = this.Data.items;
  }
}
