/* tslint:disable:member-ordering */
import { Component, EventEmitter, Injector, Output, ViewChild, ChangeDetectorRef, QueryList } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditDocumentFileDto,
  CreateOrEditTruckDto,
  DocumentFileDto,
  DocumentFilesServiceProxy,
  TrucksServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { DateType } from '@app/admin/required-document-files/hijri-gregorian-datepicker/consts';
import { DateFormatterService } from '@app/admin/required-document-files/hijri-gregorian-datepicker/date-formatter.service';
import * as _ from 'lodash';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as moment from '@node_modules/moment';
import { ViewChildren } from '@angular/core';

@Component({
  selector: 'viewOrEditEntityDocumentsModal',
  templateUrl: './view-or-edit-entity-documents-modal.componant.html',
  styleUrls: ['./view-or-edit-entity-documents-modal.componant.css'],
  providers: [DateFormatterService],
})
export class ViewOrEditEntityDocumentsModalComponent extends AppComponentBase {
  @ViewChild('viewOrEditModal', { static: true }) modal: ModalDirective;

  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChildren('dataTable') TableComponent: QueryList<Table>;
  @ViewChildren('paginator') PaginatorComponent: QueryList<Paginator>;
  private dataTable: Table;
  private paginator: Paginator;
  public ngAfterViewInit(): void {
    this.TableComponent.changes.subscribe((tComps: QueryList<Table>) => {
      this.dataTable = tComps.first;
    });
    this.PaginatorComponent.changes.subscribe((pComps: QueryList<Paginator>) => {
      this.paginator = pComps.first;
    });
  }
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
  entityType = '';
  entityId = '';
  truckId = '';
  isMissingDocumentFiles = false;
  imageChangedEvent: any = '';
  public maxProfilPictureBytesUserFriendlyValue = 5;
  public uploader: FileUploader;
  public temporaryPictureUrl: string;

  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _tokenService: TokenService,
    private _localStorageService: LocalStorageService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private dateFormatterService: DateFormatterService,
    private _fileDownloadService: FileDownloadService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(injector);
  }

  show(entityId: string, entityType: string): void {
    // this.ModalIsEdit = true;

    this.entityType = entityType;
    this.entityId = entityId;
    this.checkIfMissingDocumentFiles();
    this.active = true;
    this.modal.show();
  }

  deleteDocumentFile(documentFile: DocumentFileDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._documentFilesServiceProxy.delete(documentFile.id).subscribe(() => {
          this.reloadPage();

          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  downloadDocument(documentFile: DocumentFileDto) {
    this._documentFilesServiceProxy.getDocumentFileDto(documentFile.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  reloadPage(): void {
    this.changeDetectorRef.detectChanges();
    this.checkIfMissingDocumentFiles();
    this.paginator.changePage(this.paginator.getPage());
  }

  getDocumentFiles(event?: LazyLoadEvent) {
    this.changeDetectorRef.detectChanges();
    this.checkIfMissingDocumentFiles();
    this.changeDetectorRef.detectChanges();
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._documentFilesServiceProxy
      .getAll(
        this.filterText,
        this.maxExpirationDateFilter,
        this.minExpirationDateFilter,
        this.documentTypeDisplayNameFilter,
        this.entityType,
        this.truckId,
        this.entityId,
        this.trailerTrailerCodeFilter,
        this.userNameFilter,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }
  checkIfMissingDocumentFiles() {
    this._documentFilesServiceProxy.checkIfMissingDocumentFiles(this.entityId, this.entityType).subscribe((res) => {
      this.isMissingDocumentFiles = res;
    });
  }
}
