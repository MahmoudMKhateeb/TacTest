import { Component, Injector, Input } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { InvoiceInfoDto, InvoiceItemDto } from '@shared/service-proxies/service-proxies';

@Component({
  animations: [appModuleAnimation()],
  styleUrls: ['./invoice-template.component.scss'],
  templateUrl: './invoice-template.component.html',
  selector: 'invoice-template',
})
export class InvoiceTemplateComponent extends AppComponentBase {
  @Input() Data: InvoiceInfoDto;
  @Input() Items: InvoiceItemDto[];
  loading = false;
  constructor(injector: Injector, private route: ActivatedRoute) {
    super(injector);
    this.Data = this.route.snapshot.data.invoiceinfo;
    this.Items = this.Data.items;
  }
}
