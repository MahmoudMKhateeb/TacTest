import { Component, Injector, ViewChild, Input, OnInit, AfterViewInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  InvoiceServiceProxy,
  SubmitInvoicesServiceProxy,
  SubmitInvoiceInfoDto,
  InvoiceInfoDto,
  InvoiceItemDto,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-invoice-tenant-item',
  templateUrl: './invoice-tenant-item.component.html',
  styleUrls: ['./invoice-tenant-item.component.css'],
  animations: [appModuleAnimation()],
})
export class InvoiceTenantItemComponent extends AppComponentBase implements AfterViewInit, OnInit {
  @Input() Id: any;
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  Items: InvoiceItemDto[];
  Data: any;
  itemsDataSource: any = {};
  constructor(
    injector: Injector,
    private _invoiceService: InvoiceServiceProxy,
    private _submitInvoicesService: SubmitInvoicesServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    console.log(this.Id);
  }

  exportToExcel(): void {
    if (this.Data instanceof InvoiceInfoDto) {
      this._invoiceService.exportItems(this.Data.id).subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
    } else {
      this._submitInvoicesService.exportItems(this.Data.id).subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
    }
  }

  ngAfterViewInit(): void {}

  ngOnInit(): void {
    this.initDatasource();
  }

  initDatasource() {
    let self = this;

    this.itemsDataSource = {};
    this.itemsDataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._submitInvoicesService
          .getById(self.Id)
          .toPromise()
          .then((response) => {
            self.Data = response;
            return {
              data: response.items,
              totalAmount: response.totalAmount,
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
