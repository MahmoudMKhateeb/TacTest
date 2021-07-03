import { Component, Injector, ViewChild, Input } from '@angular/core';
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

@Component({
  templateUrl: './invoice-tenant-items-details.component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],

  selector: 'invoice-tenant-items-details',
  animations: [appModuleAnimation()],
})
export class InvoiceTenantItemsDetailsComponent extends AppComponentBase {
  @Input() Data: SubmitInvoiceInfoDto | InvoiceInfoDto | undefined = undefined;
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  direction: string;
  Items: InvoiceItemDto[];
  constructor(
    injector: Injector,
    private _invoiceService: InvoiceServiceProxy,
    private _submitInvoicesService: SubmitInvoicesServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }
  ngOnInit(): void {}
  show(data: SubmitInvoiceInfoDto | InvoiceInfoDto): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.Data = data;
    this.Items = this.Data.items;
    this.active = true;
    this.modal.show();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
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
}
