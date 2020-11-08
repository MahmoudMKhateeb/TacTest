import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentFilesServiceProxy, DocumentFileDto, GetDocumentEntitiesLookupForDocumentFilesDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditDocumentFileModalComponent } from './create-or-edit-documentFile-modal.component';

import { ViewDocumentFileModalComponent } from './view-documentFile-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './documentFiles.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class DocumentFilesComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;
  @ViewChild('viewDocumentFileModalComponent', { static: true }) viewDocumentFileModal: ViewDocumentFileModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  extnFilter = '';
  binaryObjectIdFilter = '';
  maxExpirationDateFilter: moment.Moment;
  minExpirationDateFilter: moment.Moment;
  isAcceptedFilter = false;
  documentTypeDisplayNameFilter = '';
  truckIdFilter = '';
  entityIdFilter = '';
  trailerTrailerCodeFilter = '';
  userNameFilter = '';
  isHost = true;
  routStepDisplayNameFilter = '';

  entityType = 'Tenant';
  entityTypesList: GetDocumentEntitiesLookupForDocumentFilesDto[] = [];

  _entityTypeFullName = 'TACHYON.Documents.DocumentFiles.DocumentFile';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._documentFilesServiceProxy.getIsCurrentTenantHost().subscribe((res) => {
      if (res) {
        this.isHost = true;
      } else {
        this.isHost = false;
      }
      this.entityType = 'Tenant';
    });

    this._documentFilesServiceProxy.getDocumentEntitiesForDocumentFile().subscribe((res) => {
      this.entityTypesList = res;
    });

    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
  }

  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }

  getDocumentFiles(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._documentFilesServiceProxy
      .getAll(
        this.filterText,
        // this.nameFilter,
        // this.extnFilter,
        // this.binaryObjectIdFilter,
        this.maxExpirationDateFilter,
        this.minExpirationDateFilter,
        // this.isAcceptedFilter,
        this.documentTypeDisplayNameFilter,
        this.entityType,
        this.truckIdFilter,
        this.entityIdFilter,
        this.trailerTrailerCodeFilter,
        this.userNameFilter,
        // this.routStepDisplayNameFilter,
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

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  // createDocumentFile(): void {
  //   this.createOrEditDocumentFileModal.show();
  // }

  showHistory(documentFile: DocumentFileDto): void {
    this.entityTypeHistoryModal.show({
      entityId: documentFile.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
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

  // exportToExcel(): void {
  //   this._documentFilesServiceProxy
  //     .getDocumentFilesToExcel(
  //       this.filterText,
  //       this.nameFilter,
  //       this.extnFilter,
  //       this.binaryObjectIdFilter,
  //       this.maxExpirationDateFilter,
  //       this.minExpirationDateFilter,
  //       this.isAcceptedFilter,
  //       this.documentTypeDisplayNameFilter,
  //       this.truckPlateNumberFilter,
  //       this.trailerTrailerCodeFilter,
  //       this.userNameFilter,
  //       this.routStepDisplayNameFilter
  //     )
  //     .subscribe((result) => {
  //       this._fileDownloadService.downloadTempFile(result);
  //     });
  // }

  downloadDocument(documentFile: DocumentFileDto) {
    this._documentFilesServiceProxy.getDocumentFileDto(documentFile.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  SwitchEntityType(type) {
    this.entityType = type;
    this.getDocumentFiles();
  }
}
