import { Component, OnInit, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import {
  InvoiceServiceProxy,
  InvoiceListDto,
  ISelectItemDto,
  CommonLookupServiceProxy,
  InvoiceFilterInput,
  InvoiceAccountType,
  InvoiceReportServiceServiceProxy,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InvoiceTenantItemsDetailsComponent } from 'app/main/invoices/invoice-tenants/model/invoice-tenant-items-details.component';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './invoices-list.component.html',
  encapsulation: ViewEncapsulation.None,

  animations: [appModuleAnimation()],
})
export class InvoicesListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('InvoiceDetailsModel', { static: true }) InvoiceDetailsModel: InvoiceTenantItemsDetailsComponent;

  Invoices: InvoiceListDto[] = [];
  IsStartSearch: boolean = false;
  PaidStatus: boolean | null | undefined;
  advancedFiltersAreShown = false;
  periodId: number | null | undefined;
  Periods: ISelectItemDto[];
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  dueFromDate: moment.Moment | null | undefined;
  dueToDate: moment.Moment | null | undefined;

  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;

  dueDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  duteDateRangeActive: boolean = false;
  accountType: InvoiceAccountType | undefined = undefined;
  dataSource: any = {};
  constructor(
    injector: Injector,
    private _InvoiceServiceProxy: InvoiceServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private router: Router
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.appSession.tenantId) this.advancedFiltersAreShown = true;
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
    this.getAllInvoices();
  }
  getAll(event?: LazyLoadEvent): void {
    // if (this.primengTableHelper.shouldResetPaging(event)) {
    //   this.paginator.changePage(0);
    //   return;
    // }
    //
    // this.primengTableHelper.showLoadingIndicator();
    //
    // if (this.creationDateRangeActive) {
    //   this.fromDate = moment(this.creationDateRange[0]);
    //   this.toDate = moment(this.creationDateRange[1]);
    // } else {
    //   this.fromDate = null;
    //   this.toDate = null;
    // }
    //
    // if (this.duteDateRangeActive) {
    //   this.dueFromDate = moment(this.dueDateRange[0]);
    //   this.dueToDate = moment(this.dueDateRange[1]);
    // } else {
    //   this.dueFromDate = null;
    //   this.dueToDate = null;
    // }
    // this._InvoiceServiceProxy
    //   .getAll(
    //     this.Tenant ? parseInt(this.Tenant.id) : undefined,
    //     this.periodId,
    //     this.PaidStatus,
    //     this.accountType,
    //     this.fromDate,
    //     this.toDate,
    //     this.dueFromDate,
    //     this.dueToDate,
    //     this.primengTableHelper.getSorting(this.dataTable),
    //     this.primengTableHelper.getSkipCount(this.paginator, event),
    //     this.primengTableHelper.getMaxResultCount(this.paginator, event)
    //   )
    //   .subscribe((result) => {
    //     this.IsStartSearch = true;
    //     this.primengTableHelper.totalRecordsCount = result.totalCount;
    //     this.primengTableHelper.records = result.items;
    //     this.primengTableHelper.hideLoadingIndicator();
    //     console.log(result.items);
    //   });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
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

  exportToExcel(): void {
    let data: InvoiceFilterInput = new InvoiceFilterInput();
    console.log(this.accountType);
    data.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
    data.periodId = this.periodId;
    data.isPaid = this.PaidStatus;
    data.accountType = this.accountType;
    data.fromDate = this.fromDate;
    data.toDate = this.toDate;
    data.dueFromDate = this.fromDate;
    data.dueToDate = this.toDate;
    data.sorting = this.primengTableHelper.getSorting(this.dataTable);
    this._InvoiceServiceProxy.exports(data).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  downloadReport(id: number) {
    this._InvoiceReportServiceProxy.downloadInvoiceReportPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  details(invoice: InvoiceListDto): void {
    if (invoice.accountType == InvoiceAccountType.AccountReceivable) {
      this.router.navigate([`/app/main/invoices/detail/${invoice.id}`]);
    } else {
      this._InvoiceServiceProxy.getById(invoice.id).subscribe((result) => {
        this.InvoiceDetailsModel.show(result);
      });
    }
  }

  getAllInvoices() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._InvoiceServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.items,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
