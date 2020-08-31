import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PortsServiceProxy, PortDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditPortModalComponent } from './create-or-edit-port-modal.component';

import { ViewPortModalComponent } from './view-port-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './ports.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class PortsComponent extends AppComponentBase {
  @ViewChild('createOrEditPortModal', { static: true }) createOrEditPortModal: CreateOrEditPortModalComponent;
  @ViewChild('viewPortModalComponent', { static: true }) viewPortModal: ViewPortModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  adressFilter = '';
  maxLongitudeFilter: number;
  maxLongitudeFilterEmpty: number;
  minLongitudeFilter: number;
  minLongitudeFilterEmpty: number;
  maxLatitudeFilter: number;
  maxLatitudeFilterEmpty: number;
  minLatitudeFilter: number;
  minLatitudeFilterEmpty: number;
  cityDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _portsServiceProxy: PortsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getPorts(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._portsServiceProxy
      .getAll(
        this.filterText,
        this.nameFilter,
        this.adressFilter,
        this.maxLongitudeFilter == null ? this.maxLongitudeFilterEmpty : this.maxLongitudeFilter,
        this.minLongitudeFilter == null ? this.minLongitudeFilterEmpty : this.minLongitudeFilter,
        this.maxLatitudeFilter == null ? this.maxLatitudeFilterEmpty : this.maxLatitudeFilter,
        this.minLatitudeFilter == null ? this.minLatitudeFilterEmpty : this.minLatitudeFilter,
        this.cityDisplayNameFilter,
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

  createPort(): void {
    this.createOrEditPortModal.show();
  }

  deletePort(port: PortDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._portsServiceProxy.delete(port.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._portsServiceProxy
      .getPortsToExcel(
        this.filterText,
        this.nameFilter,
        this.adressFilter,
        this.maxLongitudeFilter == null ? this.maxLongitudeFilterEmpty : this.maxLongitudeFilter,
        this.minLongitudeFilter == null ? this.minLongitudeFilterEmpty : this.minLongitudeFilter,
        this.maxLatitudeFilter == null ? this.maxLatitudeFilterEmpty : this.maxLatitudeFilter,
        this.minLatitudeFilter == null ? this.minLatitudeFilterEmpty : this.minLatitudeFilter,
        this.cityDisplayNameFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
