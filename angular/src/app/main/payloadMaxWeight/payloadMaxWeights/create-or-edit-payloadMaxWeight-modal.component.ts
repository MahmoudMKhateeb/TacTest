import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PayloadMaxWeightsServiceProxy, CreateOrEditPayloadMaxWeightDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditPayloadMaxWeightModal',
  templateUrl: './create-or-edit-payloadMaxWeight-modal.component.html',
})
export class CreateOrEditPayloadMaxWeightModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  payloadMaxWeight: CreateOrEditPayloadMaxWeightDto = new CreateOrEditPayloadMaxWeightDto();

  constructor(injector: Injector, private _payloadMaxWeightsServiceProxy: PayloadMaxWeightsServiceProxy) {
    super(injector);
  }

  show(payloadMaxWeightId?: number): void {
    if (!payloadMaxWeightId) {
      this.payloadMaxWeight = new CreateOrEditPayloadMaxWeightDto();
      this.payloadMaxWeight.id = payloadMaxWeightId;

      this.active = true;
      this.modal.show();
    } else {
      this._payloadMaxWeightsServiceProxy.getPayloadMaxWeightForEdit(payloadMaxWeightId).subscribe((result) => {
        this.payloadMaxWeight = result.payloadMaxWeight;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._payloadMaxWeightsServiceProxy
      .createOrEdit(this.payloadMaxWeight)
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
}
