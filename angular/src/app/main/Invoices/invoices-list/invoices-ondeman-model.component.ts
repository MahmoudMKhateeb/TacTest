import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { InvoiceServiceProxy, CommonLookupServiceProxy, ISelectItemDto } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'invoices-ondeman-model',
  templateUrl: './invoices-ondeman-model.component.html',
})
export class InvoiceDemandModelComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  constructor(injector: Injector, private _currentSrv: InvoiceServiceProxy, private _CommonServ: CommonLookupServiceProxy) {
    super(injector);
  }

  show(): void {
    this.Tenant = undefined;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;

    if (this.Tenant?.id) {
    } else {
      this.Tenant = undefined;
    }
    if (!this.Tenant?.id) return;

    this._currentSrv
      .onDemand(parseInt(this.Tenant.id))
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'shipper').subscribe((result) => {
      this.Tenants = result;
    });
  }
}
