import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PickingTypesServiceProxy, CreateOrEditPickingTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditPickingTypeModal',
  templateUrl: './create-or-edit-pickingType-modal.component.html',
})
export class CreateOrEditPickingTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  pickingType: CreateOrEditPickingTypeDto = new CreateOrEditPickingTypeDto();

  constructor(injector: Injector, private _pickingTypesServiceProxy: PickingTypesServiceProxy) {
    super(injector);
  }

  show(pickingTypeId?: number): void {
    if (!pickingTypeId) {
      this.pickingType = new CreateOrEditPickingTypeDto();
      this.pickingType.id = pickingTypeId;

      this.active = true;
      this.modal.show();
    } else {
      this._pickingTypesServiceProxy.getPickingTypeForEdit(pickingTypeId).subscribe((result) => {
        this.pickingType = result.pickingType;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._pickingTypesServiceProxy
      .createOrEdit(this.pickingType)
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
