import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  CommonLookupServiceProxy,
  DynamicInvoiceServiceProxy,
  InvoiceAccountType,
  InvoiceChannel,
  InvoiceFilterInput,
  InvoiceReportServiceServiceProxy,
  InvoiceServiceProxy,
  ISelectItemDto,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InvoiceTenantItemsDetailsComponent } from 'app/main/invoices/invoice-tenants/model/invoice-tenant-items-details.component';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { InvoiceDynamicModalComponent } from '@app/main/Invoices/invoices-dynamic/invoices-dynamic-modal/invoices-dynamic-modal.component';
import Swal from 'sweetalert2';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './invoices-dynamic.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: ['./invoices-dynamic.component.css'],
  animations: [appModuleAnimation()],
})
export class InvoicesDynamicComponent extends AppComponentBase implements OnInit {
  @ViewChild('InvoiceDetailsModel', { static: true }) InvoiceDetailsModel: InvoiceTenantItemsDetailsComponent;
  @ViewChild('InvoicesDynamicModal', { static: true }) InvoicesDynamicModal: InvoiceDynamicModalComponent;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;

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

  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive = false;

  dueDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  duteDateRangeActive = false;
  accountType: InvoiceAccountType | undefined = undefined;
  dataSource: any = {};

  items = [
    {
      label: 'For Shipper',
      icon: '',
      command: () => {
        this.InvoicesDynamicModal.show(1);
      },
    },
    {
      label: 'For Carrier',
      icon: '',
      command: () => {
        this.InvoicesDynamicModal.show(2);
      },
    },
  ];

  constructor(
    injector: Injector,
    private _InvoiceServiceProxy: InvoiceServiceProxy,
    private _DynamicInvoiceServiceProxy: DynamicInvoiceServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private router: Router
  ) {
    super(injector);
  }

  ngOnInit() {
    // if (this.appSession.tenantId) {
    //     this.advancedFiltersAreShown = true;
    // }
    // this._CommonServ.getPeriods().subscribe((result) => {
    //     this.Periods = result;
    // });
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

  exportToExcel(): void {
    let data: InvoiceFilterInput = new InvoiceFilterInput();
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

  details(invoice: any): void {
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
        return self._DynamicInvoiceServiceProxy
          .getAll(loadOptions.filter, '', 6, 0)
          .toPromise()
          .then((response) => {
            return {
              data: response.items,
              totalCount: response.totalCount,
              // summary: response.summary,
              // groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
      remove: (data) => {
        return this._DynamicInvoiceServiceProxy
          .delete(data.id)
          .toPromise()
          .then((res) => {
            this.notify.info(this.l('SuccessfullyDeleted'));
          })
          .catch((error) => {
            throw new Error('Data Deletion Error');
          });
        // return self._dangerousGoodTypesServiceProxy.deleteTranslation(key).toPromise();
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

  deleteItem(id) {
    Swal.fire({
      title: this.l('AreYouSure'),
      icon: 'question',
      // iconHtml: '؟',
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('Cancel'),
      showCancelButton: true,
      showCloseButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        this._InvoiceServiceProxy.dynamicInvoiceOnDemand(id).subscribe((res) => {
          this.notify.info(this.l('SubmitInvoiceGenerated'));
          this.refreshDataGrid();
        });
      }
    });
  }

  getCompanyName(data) {
    return !!data.creditCompanyName ? data.creditCompanyName : data.debitCompanyName;
  }

  editRow(item) {
    // event.cancel = true;
    const forWho = !!item.creditCompanyName ? 1 : 2;
    this.InvoicesDynamicModal.show(forWho, item.id);
    // return false;
  }

  dynamicInvoiceOnDemand(item) {
    if (isNotNullOrUndefined(item.invoiceNumber)) {
      Swal.fire({
        title: `${this.l('InvoiceNumber')} ${item.invoiceNumber}`,
        icon: 'warning',
        showCloseButton: true,
      });
      return;
    }
    Swal.fire({
      title: this.l('AreYouSure'),
      icon: 'question',
      // iconHtml: '؟',
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
      showCancelButton: true,
      showCloseButton: true,
    }).then((result) => {
      if (result.isConfirmed) {
        this._InvoiceServiceProxy.dynamicInvoiceOnDemand(item.id).subscribe((res) => {
          this.notify.info(this.l('SubmitInvoiceGenerated'));
          this.refreshDataGrid();
        });
      }
    });
  }
}
