import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  CommonLookupServiceProxy,
  InvoiceAccountType,
  InvoiceChannel,
  InvoiceFilterInput,
  InvoiceReportServiceServiceProxy,
  InvoiceServiceProxy,
  InvoiceStatus,
  ISelectItemDto,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InvoiceTenantItemsDetailsComponent } from 'app/main/invoices/invoice-tenants/model/invoice-tenant-items-details.component';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid';
import { VoidInvoiceNoteModalComponent } from '../invoice-note/invoice-note-list/void-invoice-note-modal/void-invoice-note-modal.component';
import { InvoiceSearchInputDto } from './InvoiceSearchInputDto';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';
import { exportDataGrid } from 'devextreme/excel_exporter';
import { Workbook } from 'exceljs';
import { saveAs } from 'file-saver';

@Component({
  templateUrl: './invoices-list.component.html',
  encapsulation: ViewEncapsulation.None,

  animations: [appModuleAnimation()],
})
export class InvoicesListComponent extends AppComponentBase implements OnInit {
  @ViewChild('InvoiceDetailsModel', { static: true }) InvoiceDetailsModel: InvoiceTenantItemsDetailsComponent;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild('voidModal', { static: true }) voidModal: VoidInvoiceNoteModalComponent;
  @ViewChild('sharedPdfViewer') sharedPdfViewer: FileViwerComponent;

  searchInput: InvoiceSearchInputDto = new InvoiceSearchInputDto();
  IsStartSearch = false;
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
  InvoiceChannelEnum = InvoiceChannel;
  InvoiceStatusEnum = InvoiceStatus;
  invoiceStatusTitles = Object.entries(this.InvoiceStatusEnum);
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive = false;

  dueDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  duteDateRangeActive = false;
  accountType: InvoiceAccountType | undefined = undefined;
  dataSource: any = {};
  invoiceChannels: any;

  constructor(
    injector: Injector,
    private _InvoiceServiceProxy: InvoiceServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private router: Router,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.invoiceChannels = this.enumToArray.transform(InvoiceChannel);
  }

  ngOnInit() {
    if (this.appSession.tenantId) {
      this.advancedFiltersAreShown = true;
    }
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
    this.getAllInvoices();
    console.log(this.invoiceStatusTitles);
  }

  reloadPage(): void {
    this.refreshDataGrid();
  }

  MakePaid(invoice: any): void {
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

  MakeUnPaid(invoice: any): void {
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

  searchInvoice() {
    this.getAllInvoices();
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
    this._InvoiceServiceProxy.exports(data).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  ExportItemToExcel(event: number) {
    this._InvoiceServiceProxy.exportItems(event).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  onExporting(e) {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Employees');
    worksheet.columns = [{ width: 20 }, { width: 15 }, { width: 15 }, { width: 15 }, { width: 15 }, { width: 15 }];
    exportDataGrid({
      component: e.component,
      worksheet,
      autoFilterEnabled: true,
      customizeCell: ({ gridCell, excelCell }) => {
        if (gridCell.rowType === 'data') {
          if (gridCell.column.dataField === 'isPaid') {
            excelCell.value = Boolean<any>(gridCell.value) ? this.l('Paid') : this.l('Unpaid');
          }

          if (gridCell.column.dataField === 'channel') {
            excelCell.value = InvoiceChannel[<InvoiceChannel>excelCell.value];
          }

          if (gridCell.column.dataField === 'status') {
            excelCell.value = InvoiceStatus[<InvoiceStatus>excelCell.value];
          }

          if (gridCell.column.dataField === 'accountType') {
            excelCell.value = InvoiceAccountType[<InvoiceAccountType>excelCell.value];
          }
        }

        if (gridCell.rowType === 'group') {
          excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'BEDFE6' } };
        }
      },
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'DataGrid.xlsx');
      });
    });
    e.cancel = true;
  }

  downloadReport(id: number) {
    this._InvoiceReportServiceProxy.downloadInvoiceReportPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  details(invoice: any): void {
    this._InvoiceReportServiceProxy.downloadInvoiceReportPdf(invoice.id).subscribe((result) => {
      let file = this._fileDownloadService.GetTempFileUrl(result);
      this.sharedPdfViewer.show(file, 'pdf');
    });
  }

  // if (invoice.accountType == InvoiceAccountType.AccountReceivable) {
  //  this.router.navigate([`/app/main/invoices/detail/${invoice.id}`]);
  // } else {
  //   this._InvoiceServiceProxy.getById(invoice.id).subscribe((result) => {
  //     this.InvoiceDetailsModel.show(result);
  //   });
  // }
  //}

  getAllInvoices() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._InvoiceServiceProxy
          .getAll(
            JSON.stringify(loadOptions),
            self.searchInput.paymentDate,
            self.searchInput.waybillOrSubWaybillNumber,
            self.searchInput.containerNumber,
            self.searchInput.accountNumber
          )
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

  getInvoiceStatusTitle(status) {
    let invoiceStatus = this.invoiceStatusTitles.find((x) => x[0] == status);
    return invoiceStatus[1];
  }

  confirmInvoice(invoice: any) {
    this._InvoiceServiceProxy.confirmInvoice(invoice.id).subscribe(() => {
      this.notify.success(this.l('Successfully'));
      this.getAllInvoices();
    });
  }

  /**
   * checks who can See Dynamic Invoice Label
   * @param options
   */
  canSeeDynamicInvoice(options) {
    let allowedUsers = this.isTachyonDealerOrHost || this.hasCarrierClients || this.hasShipperClients;
    if (options.data.channel == this.InvoiceChannelEnum.DynamicInvoice && allowedUsers) {
      return true;
    } else if (options.data.channel != this.InvoiceChannelEnum.DynamicInvoice) {
      return true;
    }
  }
}
