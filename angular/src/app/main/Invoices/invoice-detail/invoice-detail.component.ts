import { Component, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import {
  InvoiceChannel,
  InvoiceInfoDto,
  InvoiceItemDto,
  InvoiceReportServiceServiceProxy,
  InvoiceServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './invoice-detail.component.html',
})
export class InvoiceDetailComponent extends AppComponentBase {
  Data: InvoiceInfoDto;
  Items: InvoiceItemDto[];
  loading = false;
  @ViewChild('pdfViewerAutoLoad', { static: true }) pdfViewerAutoLoad;

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private router: Router,
    private _InvoiceServiceProxy: InvoiceServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    this.Data = this.route.snapshot.data.invoiceinfo;
    this.Items = this.Data.items;

    console.log(this.Items);
    if (this.Data.channel === InvoiceChannel.DynamicInvoice) {
      this._InvoiceReportServiceProxy.downloadDynamicInvoice(this.Data.id).subscribe((result) => {
        let url = this._fileDownloadService.GetTempFileUrl(result);
        this.downloadFile(url).subscribe((res) => {
          this.pdfViewerAutoLoad.pdfSrc = res;
          this.pdfViewerAutoLoad.refresh();
          console.log('res', res);
        });
      });
    } 
    else if (this.Data.channel === InvoiceChannel.Trip){
      this._InvoiceReportServiceProxy.downloadInvoiceReportPdf(this.Data.id).subscribe((result) => {
        let url = this._fileDownloadService.GetTempFileUrl(result);
        this.downloadFile(url).subscribe((res) => {
          this.pdfViewerAutoLoad.pdfSrc = res;
          this.pdfViewerAutoLoad.refresh();
          console.log('res', res);
        });
      });
    }
    else{
      
      this._InvoiceReportServiceProxy.downloadActorShipperInvoiceReportPdf(this.Data.id).subscribe((result) => {
        let url = this._fileDownloadService.GetTempFileUrl(result);
        this.downloadFile(url).subscribe((res) => {
          this.pdfViewerAutoLoad.pdfSrc = res;
          this.pdfViewerAutoLoad.refresh();
          console.log('res', res);
        });
      });
    }
  }

  MakePaid(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makePaid(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isPaid = true;
        });
      }
    });
  }

  MakeUnPaid(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoiceServiceProxy.makeUnPaid(this.Data.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.Data.isPaid = false;
        });
      }
    });
  }

  downloadInvoice() {
    this.loading = true;
    this._InvoiceReportServiceProxy.downloadInvoiceReportPdf(this.Data.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.loading = false;
    });
  }

  onPrint() {
    const printContents = document.getElementById('invoicecontent').innerHTML;
    //const originalContents = document.body.innerHTML;
    document.body.innerHTML = printContents;
    window.print();
    //document.body.innerHTML = originalContents;
  }
}
