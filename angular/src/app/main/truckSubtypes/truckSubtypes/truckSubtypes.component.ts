import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TruckSubtypesServiceProxy, TruckSubtypeDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckSubtypeModalComponent } from './create-or-edit-truckSubtype-modal.component';

import { ViewTruckSubtypeModalComponent } from './view-truckSubtype-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './truckSubtypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TruckSubtypesComponent extends AppComponentBase {
  @ViewChild('createOrEditTruckSubtypeModal', { static: true }) createOrEditTruckSubtypeModal: CreateOrEditTruckSubtypeModalComponent;
  @ViewChild('viewTruckSubtypeModalComponent', { static: true }) viewTruckSubtypeModal: ViewTruckSubtypeModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  trucksTypeDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _truckSubtypesServiceProxy: TruckSubtypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getTruckSubtypes(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._truckSubtypesServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
        this.trucksTypeDisplayNameFilter,
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

  createTruckSubtype(): void {
    this.createOrEditTruckSubtypeModal.show();
  }

  deleteTruckSubtype(truckSubtype: TruckSubtypeDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._truckSubtypesServiceProxy.delete(truckSubtype.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
