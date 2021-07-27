import { Component, OnInit, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import {
  SubmitInvoicesServiceProxy,
  SubmitInvoiceListDto,
  ISelectItemDto,
  CommonLookupServiceProxy,
  SubmitInvoiceFilterInput,
  SubmitInvoiceStatus,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { InvoiceTenantItemsDetailsComponent } from './model/invoice-tenant-items-details.component';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './invoice-tenant.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class InvoiceTenantComponent extends AppComponentBase implements OnInit {
  @ViewChild('InvoiceDetailsModel', { static: true }) InvoiceDetailsModel: InvoiceTenantItemsDetailsComponent;

  SubmitStatus: any;
  Invoices: SubmitInvoiceListDto[] = [];
  IsStartSearch: boolean = false;
  advancedFiltersAreShown = false;
  Periods: ISelectItemDto[];
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;

  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;

  inputSearch: SubmitInvoiceFilterInput = new SubmitInvoiceFilterInput();
  dataSource: any = {};
  constructor(
    injector: Injector,
    private _InvoiceServiceProxy: SubmitInvoicesServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.SubmitStatus = this.enumToArray.transform(SubmitInvoiceStatus);
  }

  ngOnInit() {
    if (this.appSession.tenantId) this.advancedFiltersAreShown = true;
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
    this.getAllSubmitInvoices();
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
    //   this.inputSearch.fromDate = moment(this.creationDateRange[0]);
    //   this.inputSearch.toDate = moment(this.creationDateRange[1]);
    // } else {
    //   this.fromDate = null;
    //   this.toDate = null;
    // }
    //
    // this.inputSearch.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
    // this._InvoiceServiceProxy
    //   .getAll(
    //     this.inputSearch.tenantId,
    //     this.inputSearch.periodId,
    //     this.inputSearch.fromDate,
    //     this.inputSearch.toDate,
    //     this.inputSearch.status,
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

  reloadPage(): void {}

  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'carrier').subscribe((result) => {
      this.Tenants = result;
    });
  }

  Accepted(Group: SubmitInvoiceListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.accepted(Group.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }

  StyleStatus(Status: SubmitInvoiceStatus): string {
    switch (Status) {
      case SubmitInvoiceStatus.Accepted:
        return 'label label-success label-inline m-1';
      case SubmitInvoiceStatus.Rejected:
        return 'label label-danger label-inline m-1';
      case SubmitInvoiceStatus.Claim:
        return 'label label-info label-inline m-1';
      default:
        return 'label label-default label-inline m-1';
    }
    return '';
  }

  downloadDocument(id: number): void {
    this._InvoiceServiceProxy.getFileDto(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  exportToExcel(): void {
    this.inputSearch.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
    // this.inputSearch.sorting = this.primengTableHelper.getSorting(this.dataTable);
    this._InvoiceServiceProxy.exports(this.inputSearch).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  details(invoice: SubmitInvoiceListDto): void {
    this._InvoiceServiceProxy.getById(invoice.id).subscribe((result) => {
      this.InvoiceDetailsModel.show(result);
    });
  }

  getAllSubmitInvoices() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._InvoiceServiceProxy
          .getAllSubmitInvoices(JSON.stringify(loadOptions))
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
