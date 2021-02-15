import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import * as _ from 'lodash';
import { InvoiceServiceProxy, InvoiceListDto } from '@shared/service-proxies/service-proxies';
@Component({
  templateUrl: './invoices-list.component.html',
  animations: [appModuleAnimation()],
})
export class InvoicesListComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Invoices: InvoiceListDto[] = [];
  IsStartSearch: boolean = false;
  PaidStatus: boolean | null | undefined;
  AccountStatus: boolean | null | undefined;
  filterText = '';
  advancedFiltersAreShown = false;

  constructor(injector: Injector, private _InvoiceServiceProxy: InvoiceServiceProxy) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._InvoiceServiceProxy
      .getAll(
        this.filterText,
        this.PaidStatus,
        this.AccountStatus,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.items.length;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  delete(invoice: InvoiceListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.delete(invoice.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }

  MakePaid(invoice: InvoiceListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makePaid(invoice.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }

  MakeUnPaid(invoice: InvoiceListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makeUnPaid(invoice.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }
  exportToExcel(): void {}
}
