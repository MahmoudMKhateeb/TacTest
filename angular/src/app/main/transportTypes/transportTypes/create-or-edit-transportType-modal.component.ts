import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TransportTypesServiceProxy, CreateOrEditTransportTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTransportTypeModal',
  templateUrl: './create-or-edit-transportType-modal.component.html',
})
export class CreateOrEditTransportTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  transportType: CreateOrEditTransportTypeDto = new CreateOrEditTransportTypeDto();

  constructor(injector: Injector, private _transportTypesServiceProxy: TransportTypesServiceProxy) {
    super(injector);
  }

  show(transportTypeId?: number): void {
    if (!transportTypeId) {
      this.transportType = new CreateOrEditTransportTypeDto();
      this.transportType.id = transportTypeId;

      this.active = true;
      this.modal.show();
    } else {
      this._transportTypesServiceProxy.getTransportTypeForEdit(transportTypeId).subscribe((result) => {
        this.transportType = result.transportType;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._transportTypesServiceProxy
      .createOrEdit(this.transportType)
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
