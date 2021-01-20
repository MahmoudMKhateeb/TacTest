import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { NationalityTranslationsServiceProxy, CreateOrEditNationalityTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'masterDetailChild_Nationality_createOrEditNationalityTranslationModal',
  templateUrl: './masterDetailChild_Nationality_create-or-edit-nationalityTranslation-modal.component.html',
})
export class MasterDetailChild_Nationality_CreateOrEditNationalityTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  nationalityTranslation: CreateOrEditNationalityTranslationDto = new CreateOrEditNationalityTranslationDto();

  constructor(injector: Injector, private _nationalityTranslationsServiceProxy: NationalityTranslationsServiceProxy) {
    super(injector);
  }

  nationalityId: any;

  show(nationalityId: any, nationalityTranslationId?: number): void {
    this.nationalityId = nationalityId;

    if (!nationalityTranslationId) {
      this.nationalityTranslation = new CreateOrEditNationalityTranslationDto();
      this.nationalityTranslation.id = nationalityTranslationId;

      this.active = true;
      this.modal.show();
    } else {
      this._nationalityTranslationsServiceProxy.getNationalityTranslationForEdit(nationalityTranslationId).subscribe((result) => {
        this.nationalityTranslation = result.nationalityTranslation;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this.nationalityTranslation.coreId = this.nationalityId;

    this._nationalityTranslationsServiceProxy
      .createOrEdit(this.nationalityTranslation)
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
