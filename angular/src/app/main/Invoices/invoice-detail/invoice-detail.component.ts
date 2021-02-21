import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InvoiceInfoDto } from '@shared/service-proxies/service-proxies';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './invoice-detail.component.html',
})
export class InvoiceDetailComponent extends AppComponentBase implements OnInit {
  Data: InvoiceInfoDto;
  constructor(injector: Injector, private route: ActivatedRoute, private router: Router) {
    super(injector);
    this.Data = this.route.snapshot.data.invoiceinfo;
  }

  ngOnInit(): void {}
}
