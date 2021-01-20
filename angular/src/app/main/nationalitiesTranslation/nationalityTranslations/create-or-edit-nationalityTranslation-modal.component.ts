import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  NationalityTranslationsServiceProxy,
  CreateOrEditNationalityTranslationDto,
  NationalityTranslationNationalityLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditNationalityTranslationModal',
  templateUrl: './create-or-edit-nationalityTranslation-modal.component.html',
})
export class CreateOrEditNationalityTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  nationalityTranslation: CreateOrEditNationalityTranslationDto = new CreateOrEditNationalityTranslationDto();

  nationalityName = '';

  allNationalitys: NationalityTranslationNationalityLookupTableDto[];

  constructor(injector: Injector, private _nationalityTranslationsServiceProxy: NationalityTranslationsServiceProxy) {
    super(injector);
  }

  show(nationalityTranslationId?: number): void {
    if (!nationalityTranslationId) {
      this.nationalityTranslation = new CreateOrEditNationalityTranslationDto();
      this.nationalityTranslation.id = nationalityTranslationId;
      this.nationalityName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._nationalityTranslationsServiceProxy.getNationalityTranslationForEdit(nationalityTranslationId).subscribe((result) => {
        this.nationalityTranslation = result.nationalityTranslation;

        this.nationalityName = result.nationalityName;

        this.active = true;
        this.modal.show();
      });
    }
    this._nationalityTranslationsServiceProxy.getAllNationalityForTableDropdown().subscribe((result) => {
      this.allNationalitys = result;
    });
  }

  save(): void {
    this.saving = true;

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
