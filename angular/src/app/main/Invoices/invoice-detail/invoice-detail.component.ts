import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InvoiceInfoDto, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './invoice-detail.component.html',
})
export class InvoiceDetailComponent extends AppComponentBase {
  Data: InvoiceInfoDto;
  constructor(injector: Injector, private route: ActivatedRoute, private router: Router, private _InvoiceServiceProxy: InvoiceServiceProxy) {
    super(injector);
    this.Data = this.route.snapshot.data.invoiceinfo;
  }

  delete(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.delete(this.Data.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.router.navigate([`/app/main/invoices/view`]);
        });
      }
    });
  }

  MakePaid(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makePaid(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isPaid = true;
        });
      }
    });
  }

  MakeUnPaid(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makeUnPaid(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isPaid = false;
        });
      }
    });
  }
}
