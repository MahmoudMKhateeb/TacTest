import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TrailerTypesServiceProxy, CreateOrEditTrailerTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTrailerTypeModal',
  templateUrl: './create-or-edit-trailerType-modal.component.html',
})
export class CreateOrEditTrailerTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  trailerType: CreateOrEditTrailerTypeDto = new CreateOrEditTrailerTypeDto();

  constructor(injector: Injector, private _trailerTypesServiceProxy: TrailerTypesServiceProxy) {
    super(injector);
  }

  show(trailerTypeId?: number): void {
    if (!trailerTypeId) {
      this.trailerType = new CreateOrEditTrailerTypeDto();
      this.trailerType.id = trailerTypeId;

      this.active = true;
      this.modal.show();
    } else {
      this._trailerTypesServiceProxy.getTrailerTypeForEdit(trailerTypeId).subscribe((result) => {
        this.trailerType = result.trailerType;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._trailerTypesServiceProxy
      .createOrEdit(this.trailerType)
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
