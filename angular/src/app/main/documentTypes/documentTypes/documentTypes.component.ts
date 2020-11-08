import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentTypesServiceProxy, DocumentTypeDto, GetDocumentEntitiesLookupDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditDocumentTypeModalComponent } from './create-or-edit-documentType-modal.component';

import { ViewDocumentTypeModalComponent } from './view-documentType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './documentTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class DocumentTypesComponent extends AppComponentBase {
  @ViewChild('createOrEditDocumentTypeModal', { static: true }) createOrEditDocumentTypeModal: CreateOrEditDocumentTypeModalComponent;
  @ViewChild('viewDocumentTypeModalComponent', { static: true }) viewDocumentTypeModal: ViewDocumentTypeModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  isRequiredFilter = -1;
  hasExpirationDateFilter = -1;
  requiredFromFilter = '';
  entityList: GetDocumentEntitiesLookupDto[] = [];
  constructor(
    injector: Injector,
    private _documentTypesServiceProxy: DocumentTypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getDocumentTypes(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._documentTypesServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
        this.isRequiredFilter,
        this.hasExpirationDateFilter,
        this.requiredFromFilter,
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

  createDocumentType(): void {
    this.createOrEditDocumentTypeModal.show();
  }

  deleteDocumentType(documentType: DocumentTypeDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._documentTypesServiceProxy.delete(documentType.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._documentTypesServiceProxy
      .getDocumentTypesToExcel(this.filterText, this.displayNameFilter, this.isRequiredFilter, this.hasExpirationDateFilter, this.requiredFromFilter)
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }

  getDocumentsEntityLookUp() {
    this._documentTypesServiceProxy.getDocumentEntitiesForDocumentType().subscribe((result) => {
      this.entityList = result;
    });
  }
}
