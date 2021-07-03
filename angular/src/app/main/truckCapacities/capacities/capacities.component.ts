import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CapacitiesServiceProxy, CapacityDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCapacityModalComponent } from './create-or-edit-capacity-modal.component';

import { ViewCapacityModalComponent } from './view-capacity-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './capacities.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class CapacitiesComponent extends AppComponentBase {
  @ViewChild('createOrEditCapacityModal', { static: true }) createOrEditCapacityModal: CreateOrEditCapacityModalComponent;
  @ViewChild('viewCapacityModalComponent', { static: true }) viewCapacityModal: ViewCapacityModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  truckTypeDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _capacitiesServiceProxy: CapacitiesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getCapacities(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._capacitiesServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
        this.truckTypeDisplayNameFilter,
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

  createCapacity(): void {
    this.createOrEditCapacityModal.show();
  }

  deleteCapacity(capacity: CapacityDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._capacitiesServiceProxy.delete(capacity.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
