import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  DocumentTypeTranslationsServiceProxy,
  CreateOrEditDocumentTypeTranslationDto,
  DocumentTypeTranslationDocumentTypeLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditDocumentTypeTranslationModal',
  templateUrl: './create-or-edit-documentTypeTranslation-modal.component.html',
})
export class CreateOrEditDocumentTypeTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  documentTypeTranslation: CreateOrEditDocumentTypeTranslationDto = new CreateOrEditDocumentTypeTranslationDto();

  documentTypeDisplayName = '';

  allDocumentTypes: DocumentTypeTranslationDocumentTypeLookupTableDto[];

  constructor(injector: Injector, private _documentTypeTranslationsServiceProxy: DocumentTypeTranslationsServiceProxy) {
    super(injector);
  }

  show(documentTypeTranslationId?: number, documentTypeId?: number): void {
    if (!documentTypeTranslationId) {
      this.documentTypeTranslation = new CreateOrEditDocumentTypeTranslationDto();
      this.documentTypeTranslation.id = documentTypeTranslationId;
      this.documentTypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._documentTypeTranslationsServiceProxy.getDocumentTypeTranslationForEdit(documentTypeTranslationId).subscribe((result) => {
        this.documentTypeTranslation = result.documentTypeTranslation;

        this.documentTypeDisplayName = result.documentTypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    if (documentTypeId) {
      this.documentTypeTranslation.coreId = documentTypeId;
    }

    this._documentTypeTranslationsServiceProxy.getAllDocumentTypeForTableDropdown().subscribe((result) => {
      this.allDocumentTypes = result;
    });
  }

  save(): void {
    this.saving = true;

    this._documentTypeTranslationsServiceProxy
      .createOrEdit(this.documentTypeTranslation)
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
