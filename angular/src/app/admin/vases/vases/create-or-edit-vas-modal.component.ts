import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { VasesServiceProxy, CreateOrEditVasDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditVasModal',
  templateUrl: './create-or-edit-vas-modal.component.html',
})
export class CreateOrEditVasModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  vas: CreateOrEditVasDto = new CreateOrEditVasDto();

  constructor(injector: Injector, private _vasesServiceProxy: VasesServiceProxy) {
    super(injector);
  }

  show(vasId?: number): void {
    if (!vasId) {
      this.vas = new CreateOrEditVasDto();
      this.vas.id = vasId;

      this.active = true;
      this.modal.show();
    } else {
      this._vasesServiceProxy.getVasForEdit(vasId).subscribe((result) => {
        this.vas = result.vas;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._vasesServiceProxy
      .createOrEdit(this.vas)
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
