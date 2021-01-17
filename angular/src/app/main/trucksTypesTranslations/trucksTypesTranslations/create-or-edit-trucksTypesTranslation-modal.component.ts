import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  TrucksTypesTranslationsServiceProxy,
  CreateOrEditTrucksTypesTranslationDto,
  TrucksTypesTranslationTrucksTypeLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTrucksTypesTranslationModal',
  templateUrl: './create-or-edit-trucksTypesTranslation-modal.component.html',
})
export class CreateOrEditTrucksTypesTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  trucksTypesTranslation: CreateOrEditTrucksTypesTranslationDto = new CreateOrEditTrucksTypesTranslationDto();

  trucksTypeDisplayName = '';

  allTrucksTypes: TrucksTypesTranslationTrucksTypeLookupTableDto[];

  constructor(injector: Injector, private _trucksTypesTranslationsServiceProxy: TrucksTypesTranslationsServiceProxy) {
    super(injector);
  }

  show(trucksTypesTranslationId?: number): void {
    if (!trucksTypesTranslationId) {
      this.trucksTypesTranslation = new CreateOrEditTrucksTypesTranslationDto();
      this.trucksTypesTranslation.id = trucksTypesTranslationId;
      this.trucksTypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._trucksTypesTranslationsServiceProxy.getTrucksTypesTranslationForEdit(trucksTypesTranslationId).subscribe((result) => {
        this.trucksTypesTranslation = result.trucksTypesTranslation;

        this.trucksTypeDisplayName = result.trucksTypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._trucksTypesTranslationsServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
  }

  save(): void {
    this.saving = true;

    this._trucksTypesTranslationsServiceProxy
      .createOrEdit(this.trucksTypesTranslation)
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
