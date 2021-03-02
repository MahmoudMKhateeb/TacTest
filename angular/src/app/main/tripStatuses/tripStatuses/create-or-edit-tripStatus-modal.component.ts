import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TripStatusesServiceProxy, CreateOrEditTripStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTripStatusModal',
  templateUrl: './create-or-edit-tripStatus-modal.component.html',
})
export class CreateOrEditTripStatusModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  tripStatus: CreateOrEditTripStatusDto = new CreateOrEditTripStatusDto();

  constructor(injector: Injector, private _tripStatusesServiceProxy: TripStatusesServiceProxy) {
    super(injector);
  }

  show(tripStatusId?: number): void {
    if (!tripStatusId) {
      this.tripStatus = new CreateOrEditTripStatusDto();
      this.tripStatus.id = tripStatusId;

      this.active = true;
      this.modal.show();
    } else {
      this._tripStatusesServiceProxy.getTripStatusForEdit(tripStatusId).subscribe((result) => {
        this.tripStatus = result.tripStatus;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._tripStatusesServiceProxy
      .createOrEdit(this.tripStatus)
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
