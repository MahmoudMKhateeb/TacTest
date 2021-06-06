import { Component, OnInit, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import * as _ from 'lodash';
import {
  InvoiceServiceProxy,
  InvoiceListDto,
  ISelectItemDto,
  CommonLookupServiceProxy,
  InvoiceFilterInput,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  templateUrl: './invoices-list.component.html',
  encapsulation: ViewEncapsulation.None,

  animations: [appModuleAnimation()],
})
export class InvoicesListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  Invoices: InvoiceListDto[] = [];
  IsStartSearch: boolean = false;
  PaidStatus: boolean | null | undefined;
  AccountStatus: boolean | null | undefined;
  advancedFiltersAreShown = false;
  periodId: number | null | undefined;
  Periods: ISelectItemDto[];
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  CanMakPaid: boolean;
  constructor(
    injector: Injector,
    private _InvoiceServiceProxy: InvoiceServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.appSession.tenantId) this.advancedFiltersAreShown = true;
    this.CanMakPaid = this.appSession.tenantId ? abp.features.isEnabled('app.Shipper.CanMakeInvoicePaid') : true;
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
  }
  getAll(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    if (this.creationDateRangeActive) {
      this.fromDate = moment(this.creationDateRange[0]);
      this.toDate = moment(this.creationDateRange[1]);
    } else {
      this.fromDate = null;
      this.toDate = null;
    }
    this._InvoiceServiceProxy
      .getAll(
        this.Tenant ? parseInt(this.Tenant.id) : undefined,
        this.periodId,
        this.PaidStatus,
        this.AccountStatus,
        this.fromDate,
        this.toDate,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
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
        this._InvoiceServiceProxy.makePaid(invoice.id).subscribe((r: boolean) => {
          if (r) {
            this.notify.success(this.l('Successfully'));
            this.reloadPage();
          } else {
            this.message.warn(this.l('NoEnoughBalance'));
          }
        });
      }
    });
  }

  MakeUnPaid(invoice: InvoiceListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makeUnPaid(invoice.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }
  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, '').subscribe((result) => {
      this.Tenants = result;
    });
  }
  AccountType(AccountType: boolean): string {
    return AccountType ? 'AccountReceivable' : 'AccountPayable';
  }
  exportToExcel(): void {
    var data = {
      tenantId: this.Tenant ? parseInt(this.Tenant.id) : undefined,
      periodId: this.periodId,
      isPaid: this.PaidStatus,
      isAccountReceivable: this.AccountStatus,
      fromDate: this.fromDate,
      toDate: this.toDate,
      sorting: this.primengTableHelper.getSorting(this.dataTable),
    };
    this._InvoiceServiceProxy.exports(data as InvoiceFilterInput).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
