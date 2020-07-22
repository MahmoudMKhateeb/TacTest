import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TrucksServiceProxy, TruckDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckModalComponent } from './create-or-edit-truck-modal.component';

import { ViewTruckModalComponent } from './view-truck-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './trucks.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrucksComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTruckModal', { static: true }) createOrEditTruckModal: CreateOrEditTruckModalComponent;
  @ViewChild('viewTruckModalComponent', { static: true }) viewTruckModal: ViewTruckModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  plateNumberFilter = '';
  modelNameFilter = '';
  modelYearFilter = '';
  licenseNumberFilter = '';
  maxLicenseExpirationDateFilter: moment.Moment;
  minLicenseExpirationDateFilter: moment.Moment;
  isAttachableFilter = -1;
  trucksTypeDisplayNameFilter = '';
  truckStatusDisplayNameFilter = '';
  userNameFilter = '';
  userName2Filter = '';

  _entityTypeFullName = 'TACHYON.Trucks.Truck';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
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

  getTrucks(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._trucksServiceProxy
      .getAll(
        this.filterText,
        this.plateNumberFilter,
        this.modelNameFilter,
        this.modelYearFilter,
        this.licenseNumberFilter,
        this.maxLicenseExpirationDateFilter,
        this.minLicenseExpirationDateFilter,
        this.isAttachableFilter,
        this.trucksTypeDisplayNameFilter,
        this.truckStatusDisplayNameFilter,
        this.userNameFilter,
        this.userName2Filter,
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

  createTruck(): void {
    this.createOrEditTruckModal.show();
  }

  showHistory(truck: TruckDto): void {
    this.entityTypeHistoryModal.show({
      entityId: truck.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteTruck(truck: TruckDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._trucksServiceProxy.delete(truck.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._trucksServiceProxy
      .getTrucksToExcel(
        this.filterText,
        this.plateNumberFilter,
        this.modelNameFilter,
        this.modelYearFilter,
        this.licenseNumberFilter,
        this.maxLicenseExpirationDateFilter,
        this.minLicenseExpirationDateFilter,
        this.isAttachableFilter,
        this.trucksTypeDisplayNameFilter,
        this.truckStatusDisplayNameFilter,
        this.userNameFilter,
        this.userName2Filter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
