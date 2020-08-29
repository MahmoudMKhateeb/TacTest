import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { UnitOfMeasuresServiceProxy, CreateOrEditUnitOfMeasureDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditUnitOfMeasureModal',
  templateUrl: './create-or-edit-unitOfMeasure-modal.component.html',
})
export class CreateOrEditUnitOfMeasureModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  unitOfMeasure: CreateOrEditUnitOfMeasureDto = new CreateOrEditUnitOfMeasureDto();

  constructor(injector: Injector, private _unitOfMeasuresServiceProxy: UnitOfMeasuresServiceProxy) {
    super(injector);
  }

  show(unitOfMeasureId?: number): void {
    if (!unitOfMeasureId) {
      this.unitOfMeasure = new CreateOrEditUnitOfMeasureDto();
      this.unitOfMeasure.id = unitOfMeasureId;

      this.active = true;
      this.modal.show();
    } else {
      this._unitOfMeasuresServiceProxy.getUnitOfMeasureForEdit(unitOfMeasureId).subscribe((result) => {
        this.unitOfMeasure = result.unitOfMeasure;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._unitOfMeasuresServiceProxy
      .createOrEdit(this.unitOfMeasure)
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
