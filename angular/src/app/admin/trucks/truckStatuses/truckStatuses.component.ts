import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TruckStatusesServiceProxy, TruckStatusDto, WaybillsServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckStatusModalComponent } from './create-or-edit-truckStatus-modal.component';

import { ViewTruckStatusModalComponent } from './view-truckStatus-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './truckStatuses.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TruckStatusesComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTruckStatusModal', { static: true }) createOrEditTruckStatusModal: CreateOrEditTruckStatusModalComponent;
  @ViewChild('viewTruckStatusModalComponent', { static: true }) viewTruckStatusModal: ViewTruckStatusModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  platenumber = '2121';

  _entityTypeFullName = 'TACHYON.Trucks.TruckStatus';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _truckStatusesServiceProxy: TruckStatusesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy
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

  getTruckStatuses(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._truckStatusesServiceProxy
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

  createTruckStatus(): void {
    this.createOrEditTruckStatusModal.show();
  }

  showHistory(truckStatus: TruckStatusDto): void {
    this.entityTypeHistoryModal.show({
      entityId: truckStatus.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteTruckStatus(truckStatus: TruckStatusDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._truckStatusesServiceProxy.delete(truckStatus.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
  DownloadSingleDropWaybillPdf(): void {
    this._waybillsServiceProxy.getSingleDropWaybillPdf(this.platenumber).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  DownloadMultipleDropWaybillPdf(): void {
    this._waybillsServiceProxy.getMultipleDropWaybillPdf().subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  DownloadMasterWaybillPdf(): void {
    this._waybillsServiceProxy.getMasterWaybillPdf().subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
