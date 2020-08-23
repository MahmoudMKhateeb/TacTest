import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TruckStatusesServiceProxy, CreateOrEditTruckStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTruckStatusModal',
  templateUrl: './create-or-edit-truckStatus-modal.component.html',
})
export class CreateOrEditTruckStatusModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  truckStatus: CreateOrEditTruckStatusDto = new CreateOrEditTruckStatusDto();

  constructor(injector: Injector, private _truckStatusesServiceProxy: TruckStatusesServiceProxy) {
    super(injector);
  }

  show(truckStatusId?: number): void {
    if (!truckStatusId) {
      this.truckStatus = new CreateOrEditTruckStatusDto();
      this.truckStatus.id = truckStatusId;

      this.active = true;
      this.modal.show();
    } else {
      this._truckStatusesServiceProxy.getTruckStatusForEdit(truckStatusId).subscribe((result) => {
        this.truckStatus = result.truckStatus;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._truckStatusesServiceProxy
      .createOrEdit(this.truckStatus)
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
