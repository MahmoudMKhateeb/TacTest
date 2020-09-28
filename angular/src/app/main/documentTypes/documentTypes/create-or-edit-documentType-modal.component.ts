import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  DocumentTypesServiceProxy,
  CreateOrEditDocumentTypeDto,
  DocumentFilesServiceProxy,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditDocumentTypeModal',
  templateUrl: './create-or-edit-documentType-modal.component.html',
})
export class CreateOrEditDocumentTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  documentType: CreateOrEditDocumentTypeDto = new CreateOrEditDocumentTypeDto();
  allDocumentsEntities: SelectItemDto[];

  editions: SelectItemDto[];

  constructor(
    injector: Injector,
    private _documentTypesServiceProxy: DocumentTypesServiceProxy,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy
  ) {
    super(injector);
  }

  show(documentTypeId?: number): void {
    if (!documentTypeId) {
      this.documentType = new CreateOrEditDocumentTypeDto();
      this.documentType.id = documentTypeId;
      this.documentType.expirationDate = moment().startOf('day');

      this.active = true;
      this.modal.show();
    } else {
      this._documentTypesServiceProxy.getDocumentTypeForEdit(documentTypeId).subscribe((result) => {
        this.documentType = result.documentType;

        this.active = true;
        this.modal.show();
      });
    }
    this._documentTypesServiceProxy.getAllDocumentsEntitiesForTableDropdown().subscribe((result) => {
      this.allDocumentsEntities = result;
    });
    this._documentFilesServiceProxy.getAllEditionsForDropdown().subscribe((result) => {
      this.editions = result;
    });
  }

  save(): void {
    this.saving = true;

    this._documentTypesServiceProxy
      .createOrEdit(this.documentType)
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

  getTenantSelectItemValue(): string {
    return this.allDocumentsEntities.find((x) => x.displayName === ' Tenant').id;
  }
}
