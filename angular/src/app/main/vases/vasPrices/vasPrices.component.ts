import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VasPricesServiceProxy, VasPriceDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditVasPriceModalComponent } from './create-or-edit-vasPrice-modal.component';

import { ViewVasPriceModalComponent } from './view-vasPrice-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './vasPrices.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class VasPricesComponent extends AppComponentBase {
  @ViewChild('createOrEditVasPriceModal', { static: true }) createOrEditVasPriceModal: CreateOrEditVasPriceModalComponent;
  @ViewChild('viewVasPriceModalComponent', { static: true }) viewVasPriceModal: ViewVasPriceModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  maxPriceFilter: number;
  maxPriceFilterEmpty: number;
  minPriceFilter: number;
  minPriceFilterEmpty: number;
  maxMaxAmountFilter: number;
  maxMaxAmountFilterEmpty: number;
  minMaxAmountFilter: number;
  minMaxAmountFilterEmpty: number;
  maxMaxCountFilter: number;
  maxMaxCountFilterEmpty: number;
  minMaxCountFilter: number;
  minMaxCountFilterEmpty: number;
  vasNameFilter = '';

  constructor(
    injector: Injector,
    private _vasPricesServiceProxy: VasPricesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getVasPrices(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._vasPricesServiceProxy
      .getAll(
        this.filterText,
        this.maxPriceFilter == null ? this.maxPriceFilterEmpty : this.maxPriceFilter,
        this.minPriceFilter == null ? this.minPriceFilterEmpty : this.minPriceFilter,
        this.maxMaxAmountFilter == null ? this.maxMaxAmountFilterEmpty : this.maxMaxAmountFilter,
        this.minMaxAmountFilter == null ? this.minMaxAmountFilterEmpty : this.minMaxAmountFilter,
        this.maxMaxCountFilter == null ? this.maxMaxCountFilterEmpty : this.maxMaxCountFilter,
        this.minMaxCountFilter == null ? this.minMaxCountFilterEmpty : this.minMaxCountFilter,
        this.vasNameFilter,
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

  createVasPrice(): void {
    this.createOrEditVasPriceModal.show();
  }

  deleteVasPrice(vasPrice: VasPriceDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._vasPricesServiceProxy.delete(vasPrice.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._vasPricesServiceProxy
      .getVasPricesToExcel(
        this.filterText,
        this.maxPriceFilter == null ? this.maxPriceFilterEmpty : this.maxPriceFilter,
        this.minPriceFilter == null ? this.minPriceFilterEmpty : this.minPriceFilter,
        this.maxMaxAmountFilter == null ? this.maxMaxAmountFilterEmpty : this.maxMaxAmountFilter,
        this.minMaxAmountFilter == null ? this.minMaxAmountFilterEmpty : this.minMaxAmountFilter,
        this.maxMaxCountFilter == null ? this.maxMaxCountFilterEmpty : this.maxMaxCountFilter,
        this.minMaxCountFilter == null ? this.minMaxCountFilterEmpty : this.minMaxCountFilter,
        this.vasNameFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
