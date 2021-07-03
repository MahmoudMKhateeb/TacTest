import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ShippingRequestStatusesServiceProxy, ShippingRequestStatusDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditShippingRequestStatusModalComponent } from './create-or-edit-shippingRequestStatus-modal.component';

import { ViewShippingRequestStatusModalComponent } from './view-shippingRequestStatus-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './shippingRequestStatuses.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class ShippingRequestStatusesComponent extends AppComponentBase {
  @ViewChild('createOrEditShippingRequestStatusModal', { static: true })
  createOrEditShippingRequestStatusModal: CreateOrEditShippingRequestStatusModalComponent;
  @ViewChild('viewShippingRequestStatusModalComponent', { static: true }) viewShippingRequestStatusModal: ViewShippingRequestStatusModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';

  constructor(
    injector: Injector,
    private _shippingRequestStatusesServiceProxy: ShippingRequestStatusesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getShippingRequestStatuses(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._shippingRequestStatusesServiceProxy
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

  createShippingRequestStatus(): void {
    this.createOrEditShippingRequestStatusModal.show();
  }

  deleteShippingRequestStatus(shippingRequestStatus: ShippingRequestStatusDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._shippingRequestStatusesServiceProxy.delete(shippingRequestStatus.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
