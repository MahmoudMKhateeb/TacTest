import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  DocumentFilesServiceProxy,
  CreateOrEditDocumentFileDto,
  DocumentFileDocumentTypeLookupTableDto,
  DocumentFileTruckLookupTableDto,
  DocumentFileTrailerLookupTableDto,
  DocumentFileUserLookupTableDto,
  DocumentFileRoutStepLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditDocumentFileModal',
  templateUrl: './create-or-edit-documentFile-modal.component.html',
})
export class CreateOrEditDocumentFileModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  documentFile: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();

  documentTypeDisplayName = '';
  truckPlateNumber = '';
  trailerTrailerCode = '';
  userName = '';
  routStepDisplayName = '';

  allDocumentTypes: DocumentFileDocumentTypeLookupTableDto[];
  allTrucks: DocumentFileTruckLookupTableDto[];
  allTrailers: DocumentFileTrailerLookupTableDto[];
  allUsers: DocumentFileUserLookupTableDto[];
  allRoutSteps: DocumentFileRoutStepLookupTableDto[];

  constructor(injector: Injector, private _documentFilesServiceProxy: DocumentFilesServiceProxy) {
    super(injector);
  }

  show(documentFileId?: string): void {
    if (!documentFileId) {
      this.documentFile = new CreateOrEditDocumentFileDto();
      this.documentFile.id = documentFileId;
      this.documentFile.expirationDate = moment().startOf('day');
      this.documentTypeDisplayName = '';
      this.truckPlateNumber = '';
      this.trailerTrailerCode = '';
      this.userName = '';
      this.routStepDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._documentFilesServiceProxy.getDocumentFileForEdit(documentFileId).subscribe((result) => {
        this.documentFile = result.documentFile;

        this.documentTypeDisplayName = result.documentTypeDisplayName;
        this.truckPlateNumber = result.truckPlateNumber;
        this.trailerTrailerCode = result.trailerTrailerCode;
        this.userName = result.userName;
        this.routStepDisplayName = result.routStepDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._documentFilesServiceProxy.getAllDocumentTypeForTableDropdown().subscribe((result) => {
      this.allDocumentTypes = result;
    });
    this._documentFilesServiceProxy.getAllTruckForTableDropdown().subscribe((result) => {
      this.allTrucks = result;
    });
    this._documentFilesServiceProxy.getAllTrailerForTableDropdown().subscribe((result) => {
      this.allTrailers = result;
    });
    this._documentFilesServiceProxy.getAllUserForTableDropdown().subscribe((result) => {
      this.allUsers = result;
    });
    this._documentFilesServiceProxy.getAllRoutStepForTableDropdown().subscribe((result) => {
      this.allRoutSteps = result;
    });
  }

  save(): void {
    this.saving = true;

    this._documentFilesServiceProxy
      .createOrEdit(this.documentFile)
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
