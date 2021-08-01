/* tslint:disable:member-ordering */
import { ChangeDetectorRef, Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DocumentFilesServiceProxy, DocumentsEntitiesEnum, TrucksServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenService } from '@node_modules/abp-ng2-module';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { DriverSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/drivers-submitted-documents/driver-submited-documents-list/driver-submited-documents-list.component';

@Component({
  selector: 'viewOrEditEntityDocumentsModal',
  templateUrl: './view-or-edit-entity-documents-modal.componant.html',
  styleUrls: ['./view-or-edit-entity-documents-modal.componant.css'],
  providers: [DateFormatterService],
})
export class ViewOrEditEntityDocumentsModalComponent extends AppComponentBase {
  @ViewChild('viewOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('driverSubmitedDocumentsListComponent', { static: true }) driverSubmitedDocumentsListComponent: DriverSubmitedDocumentsListComponent;

  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  ModalIsEdit = null;
  entityHistoryEnabled = false;
  entityIdFilter = '';
  isMissingDocumentFiles = false;
  public documentEntityFilter: DocumentsEntitiesEnum;
  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _tokenService: TokenService,
    private _localStorageService: LocalStorageService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  show(id: string, documentEntityFilter: DocumentsEntitiesEnum): void {
    this.documentEntityFilter = documentEntityFilter;
    this.entityIdFilter = id;

    this.driverSubmitedDocumentsListComponent.getDocumentFilesByDriverId(id);

    this.checkIfMissingDocumentFiles();
    this.active = true;
    this.modal.show();
  }

  reload() {
    this.driverSubmitedDocumentsListComponent.getDocumentFilesByDriverId(this.entityIdFilter);
  }

  close(): void {
    this.active = false;
    this.modalSave.emit(null);
    this.modal.hide();
  }

  checkIfMissingDocumentFiles() {
    this._documentFilesServiceProxy.checkIfMissingDocumentFiles(this.entityIdFilter, this.documentEntityFilter).subscribe((res) => {
      this.isMissingDocumentFiles = res;
    });
  }
}
