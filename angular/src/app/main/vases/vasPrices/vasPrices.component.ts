import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  VasPricesServiceProxy,
  VasPriceDto,
  CreateOrEditVasPriceDto,
  VasPriceVasLookupTableDto,
  VasesServiceProxy,
} from '@shared/service-proxies/service-proxies';
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
import { MessageService } from 'primeng/api';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './vasPrices.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
  providers: [MessageService],
  styles: [
    `
      :host ::ng-deep .p-cell-editing {
        padding-top: 0 !important;
        padding-bottom: 0 !important;
      }
    `,
  ],
})
export class VasPricesComponent extends AppComponentBase {
  @ViewChild('createOrEditVasPriceModal', { static: true }) createOrEditVasPriceModal: CreateOrEditVasPriceModalComponent;
  @ViewChild('viewVasPriceModalComponent', { static: true }) viewVasPriceModal: ViewVasPriceModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  vasPrice: CreateOrEditVasPriceDto = new CreateOrEditVasPriceDto();

  VasPriceDto2: CreateOrEditVasPriceDto[];

  vasList: { [s: number]: CreateOrEditVasPriceDto } = {};

  saving = false;

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
  hasAmountFilter: number;
  hasAmountFilterEmpty: number;
  hasCountFilter: number;
  hasCountFilterEmpty: number;
  vasNameFilter = '';
  vasName = '';
  allVass: VasPriceVasLookupTableDto[];

  constructor(
    injector: Injector,
    private _vasPricesServiceProxy: VasPricesServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private messageService: MessageService
  ) {
    super(injector);
  }

  onRowEditInit(vaspriceCreateEdit) {
    this.vasList[vaspriceCreateEdit.id] = { ...vaspriceCreateEdit };

    // console.log(this.vasList[vaspriceCreateEdit.id]);
  }

  onRowEditSave(vasPriceDto: VasPriceDto) {
    if (vasPriceDto.price > 0) {
      this.vasPrice.id = vasPriceDto.id == 0 ? null : vasPriceDto.id;
      this.vasPrice.vasId = vasPriceDto.vasId;
      this.vasPrice.price = vasPriceDto.price;
      this.vasPrice.maxAmount = vasPriceDto.maxAmount;
      this.vasPrice.maxCount = vasPriceDto.maxCount;

      this.save();
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Vas Details is updated' });
    } else {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Invalid Price' });
    }
  }

  onRowEditCancel(vasPrice: CreateOrEditVasPriceDto, index: number) {
    this.VasPriceDto2[index] = this.vasList[vasPrice.id];
    delete this.vasList[vasPrice.id];
  }

  save(): void {
    this.saving = true;

    this._vasPricesServiceProxy
      .createOrEdit(this.vasPrice)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        // this.close();
        // this.modalSave.emit(null);
      });
  }

  getVasPrices(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._vasPricesServiceProxy
      .getAllVASs(
        this.filterText,
        this.maxPriceFilter == null ? this.maxPriceFilterEmpty : this.maxPriceFilter,
        this.minPriceFilter == null ? this.minPriceFilterEmpty : this.minPriceFilter,
        this.maxMaxAmountFilter == null ? this.maxMaxAmountFilterEmpty : this.maxMaxAmountFilter,
        this.minMaxAmountFilter == null ? this.minMaxAmountFilterEmpty : this.minMaxAmountFilter,
        this.maxMaxCountFilter == null ? this.maxMaxCountFilterEmpty : this.maxMaxCountFilter,
        this.minMaxCountFilter == null ? this.minMaxCountFilterEmpty : this.minMaxCountFilter,
        this.hasAmountFilter == null ? this.hasAmountFilterEmpty : this.hasAmountFilter,
        this.hasCountFilter == null ? this.hasCountFilterEmpty : this.hasCountFilter,
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

  // show(vasPriceId?: number): void {
  //   // if (!vasPriceId) {
  //   //   this.VasPrice = new CreateOrEditVasPriceDto();
  //   //   this.VasPrice.id = vasPriceId;
  //   //   this.vasName = '';

  //   //   this.active = true;
  //   //   this.modal.show();
  //   // } else
  //   // {
  //   //   this._vasPricesServiceProxy.getVasPriceForEdit(vasPriceId).subscribe((result) => {
  //   //     this.vasPrice = result.vasPrice;

  //   //     this.vasName = result.vasName;

  //   //     // this.active = true;
  //   //     // this.modal.show();
  //   //   });
  //   // }

  // }

  getAllVASs() {
    this._vasPricesServiceProxy.getAllVasForTableDropdown().subscribe((result) => {
      this.allVass = result;
    });
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
