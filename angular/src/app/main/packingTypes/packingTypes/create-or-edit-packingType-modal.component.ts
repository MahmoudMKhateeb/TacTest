import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PackingTypesServiceProxy, CreateOrEditPackingTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditPackingTypeModal',
  templateUrl: './create-or-edit-packingType-modal.component.html',
})
export class CreateOrEditPackingTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  packingType: CreateOrEditPackingTypeDto = new CreateOrEditPackingTypeDto();

  constructor(injector: Injector, private _packingTypesServiceProxy: PackingTypesServiceProxy) {
    super(injector);
  }

  show(packingTypeId?: number): void {
    if (!packingTypeId) {
      this.packingType = new CreateOrEditPackingTypeDto();
      this.packingType.id = packingTypeId;

      this.active = true;
      this.modal.show();
    } else {
      this._packingTypesServiceProxy.getPackingTypeForEdit(packingTypeId).subscribe((result) => {
        this.packingType = result.packingType;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._packingTypesServiceProxy
      .createOrEdit(this.packingType)
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
