import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TermAndConditionsServiceProxy, TermAndConditionDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTermAndConditionModalComponent } from './create-or-edit-termAndCondition-modal.component';

import { ViewTermAndConditionModalComponent } from './view-termAndCondition-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './termAndConditions.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TermAndConditionsComponent extends AppComponentBase {
  @ViewChild('createOrEditTermAndConditionModal', { static: true }) createOrEditTermAndConditionModal: CreateOrEditTermAndConditionModalComponent;
  @ViewChild('viewTermAndConditionModalComponent', { static: true }) viewTermAndConditionModal: ViewTermAndConditionModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  titleFilter = '';
  contentFilter = '';
  maxVersionFilter: number;
  maxVersionFilterEmpty: number;
  minVersionFilter: number;
  minVersionFilterEmpty: number;
  maxEditionIdFilter: number;
  maxEditionIdFilterEmpty: number;
  minEditionIdFilter: number;
  minEditionIdFilterEmpty: number;

  constructor(
    injector: Injector,
    private _termAndConditionsServiceProxy: TermAndConditionsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getTermAndConditions(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._termAndConditionsServiceProxy
      .getAll(
        this.filterText,
        this.titleFilter,
        this.contentFilter,
        this.maxVersionFilter == null ? this.maxVersionFilterEmpty : this.maxVersionFilter,
        this.minVersionFilter == null ? this.minVersionFilterEmpty : this.minVersionFilter,
        this.maxEditionIdFilter == null ? this.maxEditionIdFilterEmpty : this.maxEditionIdFilter,
        this.minEditionIdFilter == null ? this.minEditionIdFilterEmpty : this.minEditionIdFilter,
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

  createTermAndCondition(): void {
    this.createOrEditTermAndConditionModal.show();
  }

  deleteTermAndCondition(termAndCondition: TermAndConditionDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._termAndConditionsServiceProxy.delete(termAndCondition.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  setTermAsActive(id: number): void {
    this._termAndConditionsServiceProxy.setAsActive(id).subscribe(() => {
      this.reloadPage();
      this.notify.success(this.l('SuccessfullyActivated'));
    });
  }
  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
}
