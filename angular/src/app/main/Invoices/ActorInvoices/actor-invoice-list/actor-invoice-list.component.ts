import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { InvoiceTenantItemsDetailsComponent } from '@app/main/Invoices/invoice-tenants/model/invoice-tenant-items-details.component';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import {
  ActorInvoiceServiceProxy,
  CommonLookupServiceProxy,
  InvoiceAccountType,
  InvoiceReportServiceServiceProxy,
  ISelectItemDto,
} from '@shared/service-proxies/service-proxies';
import * as moment from '@node_modules/moment';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';

@Component({
  selector: 'app-actor-invoice-list',
  templateUrl: './actor-invoice-list.component.html',
  styleUrls: ['./actor-invoice-list.component.css'],
})
export class ActorInvoiceListComponent extends AppComponentBase implements OnInit {
  @ViewChild('InvoiceDetailsModel', { static: true }) InvoiceDetailsModel: InvoiceTenantItemsDetailsComponent;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild('sharedPdfViewer') sharedPdfViewer: FileViwerComponent;
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

  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive = false;

  dueDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  duteDateRangeActive = false;
  accountType: InvoiceAccountType | undefined = undefined;
  dataSource: any = {};
  constructor(
    injector: Injector,
    private _InvoiceServiceProxy: ActorInvoiceServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private router: Router
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.appSession.tenantId) {
      this.advancedFiltersAreShown = true;
    }
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
    this.getAllInvoices();
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
            this.message.warn(this.l('[Failed]'));
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

  exportToExcel(): void {
    // let data: InvoiceFilterInput = new InvoiceFilterInput();
    // console.log(this.accountType);
    // data.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
    // data.periodId = this.periodId;
    // data.isPaid = this.PaidStatus;
    // data.accountType = this.accountType;
    // data.fromDate = this.fromDate;
    // data.toDate = this.toDate;
    // data.dueFromDate = this.fromDate;
    // data.dueToDate = this.toDate;
    // this._InvoiceServiceProxy.exports(data).subscribe((result) => {
    //   this._fileDownloadService.downloadTempFile(result);
    // });
  }

  downloadReport(id: number) {
    this._InvoiceReportServiceProxy.downloadActorShipperInvoiceReportPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  details(invoiceId: any): void {
    this._InvoiceReportServiceProxy.downloadActorShipperInvoiceReportPdf(invoiceId).subscribe((result) => {
      this.sharedPdfViewer.show(this._fileDownloadService.downloadTempFile(result), 'pdf');
    });
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
