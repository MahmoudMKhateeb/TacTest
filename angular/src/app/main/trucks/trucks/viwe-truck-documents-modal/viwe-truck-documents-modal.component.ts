/* tslint:disable:member-ordering */
import { ChangeDetectorRef, Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DocumentFilesServiceProxy, DocumentsEntitiesEnum, TrucksServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileUploader } from '@node_modules/ng2-file-upload';
import { TokenService } from '@node_modules/abp-ng2-module';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { LazyLoadEvent } from 'primeng/api';

import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as moment from '@node_modules/moment';
import { DriverSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/drivers-submitted-documents/driver-submited-documents-list/driver-submited-documents-list.component';
import { TruckSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/trucks-submitted-documents/truck-submited-documents-list/truck-submited-documents-list.component';

@Component({
  selector: 'app-viwe-truck-documents-modal',
  templateUrl: './viwe-truck-documents-modal.component.html',
  styleUrls: ['./viwe-truck-documents-modal.component.css'],
  providers: [DateFormatterService],
})
export class ViweTruckDocumentsModalComponent extends AppComponentBase {
  @ViewChild('viewOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('truckSubmitedDocumentsListComponent', { static: true }) truckSubmitedDocumentsListComponent: TruckSubmitedDocumentsListComponent;

  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('paginator', { static: false }) paginator: Paginator;
  @ViewChild('dataTable', { static: false }) dataTable: Table;

  active = false;
  saving = false;
  ModalIsEdit = null;
  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  extnFilter = '';
  binaryObjectIdFilter = '';
  maxExpirationDateFilter: moment.Moment;
  minExpirationDateFilter: moment.Moment;
  isAcceptedFilter = false;
  documentTypeDisplayNameFilter = '';
  truckPlateNumberFilter = '';
  trailerTrailerCodeFilter = '';
  userNameFilter = '';
  routStepDisplayNameFilter = '';
  _entityTypeFullName = 'TACHYON.Documents.DocumentFiles.DocumentFile';
  entityHistoryEnabled = false;
  testCond = null;
  entityIdFilter = '';
  truckIdFilter: number;
  isMissingDocumentFiles = false;
  imageChangedEvent: any = '';
  public maxProfilPictureBytesUserFriendlyValue = 5;
  public uploader: FileUploader;
  public temporaryPictureUrl: string;
  public documentEntityFilter: DocumentsEntitiesEnum;
  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _tokenService: TokenService,
    private _localStorageService: LocalStorageService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(injector);
  }

  show(id: string, documentEntityFilter: DocumentsEntitiesEnum): void {
    // this.ModalIsEdit = true;

    this.documentEntityFilter = documentEntityFilter;
    this.entityIdFilter = id;

    this.truckSubmitedDocumentsListComponent.getDocumentFilesByTruckId(id);
    this.checkIfMissingDocumentFiles();
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modalSave.emit(null);
    this.modal.hide();
    this.truckSubmitedDocumentsListComponent.getDocumentFilesByTruckId(this.entityIdFilter);
  }

  getDocumentFiles(event?: LazyLoadEvent) {
    // this.checkIfMissingDocumentFiles();
    // this.changeDetectorRef.detectChanges();
    // if (this.primengTableHelper.shouldResetPaging(event)) {
    //   this.paginator.changePage(0);
    //   return;
    // }
    //
    // this.primengTableHelper.showLoadingIndicator();
  }
  checkIfMissingDocumentFiles() {
    this._documentFilesServiceProxy.checkIfMissingDocumentFiles(this.entityIdFilter, this.documentEntityFilter).subscribe((res) => {
      this.isMissingDocumentFiles = res;
    });
  }
}
