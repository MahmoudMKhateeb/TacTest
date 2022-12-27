﻿import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RegionDto, RegionsServiceProxy, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditRegionModalComponent } from './create-or-edit-region-modal.component';

import { ViewRegionModalComponent } from './view-region-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  templateUrl: './regions.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class RegionsComponent extends AppComponentBase {
  @ViewChild('createOrEditRegionModal', { static: true }) createOrEditRegionModal: CreateOrEditRegionModalComponent;
  @ViewChild('viewRegionModalComponent', { static: true }) viewRegionModal: ViewRegionModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  maxBayanIntegrationIdFilter: number;
  maxBayanIntegrationIdFilterEmpty: number;
  minBayanIntegrationIdFilter: number;
  minBayanIntegrationIdFilterEmpty: number;
  countyDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _regionsServiceProxy: RegionsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getRegions(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();

    this._regionsServiceProxy
      .getAll(
        this.filterText,
        this.nameFilter,
        this.maxBayanIntegrationIdFilter == null ? this.maxBayanIntegrationIdFilterEmpty : this.maxBayanIntegrationIdFilter,
        this.minBayanIntegrationIdFilter == null ? this.minBayanIntegrationIdFilterEmpty : this.minBayanIntegrationIdFilter,
        this.countyDisplayNameFilter,
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

  createRegion(): void {
    this.createOrEditRegionModal.show();
  }

  deleteRegion(region: RegionDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._regionsServiceProxy.delete(region.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
