import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import {
  BalanceRechargeServiceProxy,
  CommonLookupServiceProxy,
  CreateBalanceRechargeInput,
  ISelectItemDto,
} from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'balance-recharge-model',
  templateUrl: './balance-recharge-model.component.html',
})
export class BalanceRechargeModelComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  Recharge: CreateBalanceRechargeInput = new CreateBalanceRechargeInput();
  constructor(injector: Injector, private _RechargeService: BalanceRechargeServiceProxy, private _CommonServ: CommonLookupServiceProxy) {
    super(injector);
  }

  show(): void {
    this.Recharge.amount = 1;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
    this.Recharge.tenantId = parseInt(this.Tenant.id);
    this._RechargeService
      .create(this.Recharge)
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
