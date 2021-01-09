import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CreateOrEditTransportTypesTranslationDto,
  TransportTypesTranslationsServiceProxy,
  TransportTypesTranslationTransportTypeLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'createOrEditTransportTypesTranslationModal',
  templateUrl: './create-or-edit-transportTypesTranslation-modal.component.html',
})
export class CreateOrEditTransportTypesTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  transportTypesTranslation: CreateOrEditTransportTypesTranslationDto = new CreateOrEditTransportTypesTranslationDto();

  transportTypeDisplayName = '';

  allTransportTypes: TransportTypesTranslationTransportTypeLookupTableDto[];

  constructor(injector: Injector, private _transportTypesTranslationsServiceProxy: TransportTypesTranslationsServiceProxy) {
    super(injector);
  }

  show(transportTypesTranslationId?: number): void {
    if (!transportTypesTranslationId) {
      this.transportTypesTranslation = new CreateOrEditTransportTypesTranslationDto();
      this.transportTypesTranslation.id = transportTypesTranslationId;
      this.transportTypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._transportTypesTranslationsServiceProxy.getTransportTypesTranslationForEdit(transportTypesTranslationId).subscribe((result) => {
        this.transportTypesTranslation = result.transportTypesTranslation;

        this.transportTypeDisplayName = result.transportTypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._transportTypesTranslationsServiceProxy.getAllTransportTypeForTableDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });
  }

  save(): void {
    this.saving = true;

    this._transportTypesTranslationsServiceProxy
      .createOrEdit(this.transportTypesTranslation)
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
