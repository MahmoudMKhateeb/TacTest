import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BayanIntegrationResultDto, BayanIntegrationResultsServiceProxy, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditBayanIntegrationResultModalComponent } from './create-or-edit-bayanIntegrationResult-modal.component';

import { ViewBayanIntegrationResultModalComponent } from './view-bayanIntegrationResult-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  templateUrl: './bayanIntegrationResults.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class BayanIntegrationResultsComponent extends AppComponentBase {
  @ViewChild('createOrEditBayanIntegrationResultModal', { static: true })
  createOrEditBayanIntegrationResultModal: CreateOrEditBayanIntegrationResultModalComponent;
  @ViewChild('viewBayanIntegrationResultModalComponent', { static: true }) viewBayanIntegrationResultModal: ViewBayanIntegrationResultModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  actionNameFilter = '';
  inputJsonFilter = '';
  responseJsonFilter = '';
  versionFilter = '';
  shippingRequestTripContainerNumberFilter = '';

  constructor(
    injector: Injector,
    private _bayanIntegrationResultsServiceProxy: BayanIntegrationResultsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getBayanIntegrationResults(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._bayanIntegrationResultsServiceProxy
      .getAll(
        this.filterText,
        this.actionNameFilter,
        this.inputJsonFilter,
        this.responseJsonFilter,
        this.versionFilter,
        this.shippingRequestTripContainerNumberFilter,
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

  createBayanIntegrationResult(): void {
    this.createOrEditBayanIntegrationResultModal.show();
  }

  deleteBayanIntegrationResult(bayanIntegrationResult: BayanIntegrationResultDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._bayanIntegrationResultsServiceProxy.delete(bayanIntegrationResult.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._bayanIntegrationResultsServiceProxy
      .getBayanIntegrationResultsToExcel(
        this.filterText,
        this.actionNameFilter,
        this.inputJsonFilter,
        this.responseJsonFilter,
        this.versionFilter,
        this.shippingRequestTripContainerNumberFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
