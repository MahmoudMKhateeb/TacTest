import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  TenantCarrierServiceProxy,
  CommonLookupServiceProxy,
  ISelectItemDto,
  CreateTenantCarrierInput,
} from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
@Component({
  templateUrl: './create-tenant-carriers-model.component.html',
  selector: 'create-tenant-carriers-model',
  animations: [appModuleAnimation()],
})
export class CreateTenantCarriersModel extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  Input: CreateTenantCarrierInput = new CreateTenantCarrierInput();

  constructor(injector: Injector, private _CurrentServ: TenantCarrierServiceProxy, private _CommonServ: CommonLookupServiceProxy) {
    super(injector);
  }

  show(id: number): void {
    this.active = true;
    this.modal.show();
    this.Input.tenantId = id;
    this.Tenant = undefined;
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'carrier').subscribe((result) => {
      this.Tenants = result;
    });
  }

  save(): void {
    this.saving = true;

    if (this.Tenant?.id) {
      this.Input.carrierTenantId = parseInt(this.Tenant.id);
    } else {
      this.Tenant = undefined;
      this.Input.carrierTenantId = undefined;
    }

    this._CurrentServ
      .create(this.Input)
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
}
