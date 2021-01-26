import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PlateTypesServiceProxy, CreateOrEditPlateTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditPlateTypeModal',
  templateUrl: './create-or-edit-plateType-modal.component.html',
})
export class CreateOrEditPlateTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  plateType: CreateOrEditPlateTypeDto = new CreateOrEditPlateTypeDto();

  constructor(injector: Injector, private _plateTypesServiceProxy: PlateTypesServiceProxy) {
    super(injector);
  }

  show(plateTypeId?: number): void {
    if (!plateTypeId) {
      this.plateType = new CreateOrEditPlateTypeDto();
      this.plateType.id = plateTypeId;

      this.active = true;
      this.modal.show();
    } else {
      this._plateTypesServiceProxy.getPlateTypeForEdit(plateTypeId).subscribe((result) => {
        this.plateType = result.plateType;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._plateTypesServiceProxy
      .createOrEdit(this.plateType)
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
