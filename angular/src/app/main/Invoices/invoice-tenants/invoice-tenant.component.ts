import { Component, OnInit, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import {
  SubmitInvoicesServiceProxy,
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
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';

@Component({
  templateUrl: './invoice-tenant.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class InvoiceTenantComponent extends AppComponentBase implements OnInit {
  @ViewChild('InvoiceDetailsModel', { static: true }) InvoiceDetailsModel: InvoiceTenantItemsDetailsComponent;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild('fileViwerComponent', { static: false }) fileViwerComponent: FileViwerComponent;

  SubmitStatus: any;
  IsStartSearch = false;
  advancedFiltersAreShown = false;
  Periods: ISelectItemDto[];
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;

  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive = false;

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
    if (this.appSession.tenantId) {
      this.advancedFiltersAreShown = true;
    }
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
    this.getAllSubmitInvoices();
  }
  reloadPage(): void {
    this.refreshDataGrid();
  }
  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'carrier').subscribe((result) => {
      this.Tenants = result;
    });
  }

  MakePaid(invoice: any): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makeSubmitInvoicePaid(invoice.id).subscribe((r: boolean) => {
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
  MakeUnPaid(invoice: any): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makeSubmitInvoiceUnPaid(invoice.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }

  Accepted(id): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.accepted(id).subscribe(() => {
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
      this.fileViwerComponent.show(this._fileDownloadService.downloadTempFile(result), 'pdf');
    });
  }

  exportToExcel(): void {
    this.inputSearch.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
    // this.inputSearch.sorting = this.primengTableHelper.getSorting(this.dataTable);
    this._InvoiceServiceProxy.exports(this.inputSearch).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  details(id): void {
    this._InvoiceServiceProxy.getById(id).subscribe((result) => {
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
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
  refreshDataGrid() {
    this.dataGrid.instance
      .refresh()
      .then(function () {
        // ...
      })
      .catch(function (error) {
        // ...
      });
  }
}
