import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TrailerStatusesServiceProxy, TrailerStatusDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTrailerStatusModalComponent } from './create-or-edit-trailerStatus-modal.component';

import { ViewTrailerStatusModalComponent } from './view-trailerStatus-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './trailerStatuses.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrailerStatusesComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTrailerStatusModal', { static: true }) createOrEditTrailerStatusModal: CreateOrEditTrailerStatusModalComponent;
  @ViewChild('viewTrailerStatusModalComponent', { static: true }) viewTrailerStatusModal: ViewTrailerStatusModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';

  _entityTypeFullName = 'TACHYON.Trailers.TrailerStatuses.TrailerStatus';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _trailerStatusesServiceProxy: TrailerStatusesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
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

  getTrailerStatuses(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._trailerStatusesServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
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

  createTrailerStatus(): void {
    this.createOrEditTrailerStatusModal.show();
  }

  showHistory(trailerStatus: TrailerStatusDto): void {
    this.entityTypeHistoryModal.show({
      entityId: trailerStatus.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteTrailerStatus(trailerStatus: TrailerStatusDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._trailerStatusesServiceProxy.delete(trailerStatus.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._trailerStatusesServiceProxy.getTrailerStatusesToExcel(this.filterText, this.displayNameFilter).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
