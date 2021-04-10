import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InvoiceReportServiceServiceProxy, WaybillsServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { result } from 'lodash';

@Component({
  templateUrl: './waybills.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class waybillsComponent extends AppComponentBase {
  constructor(
    injector: Injector,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _InvoiceReportServiceProxy: InvoiceReportServiceServiceProxy
  ) {
    super(injector);
  }

  platenumber = '2121';

  DownloadSingleDropWaybillPdf(shippingRequestTripId: number): void {
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(shippingRequestTripId).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  DownloadMultipleDropWaybillPdf(pointId: number): void {
    this._waybillsServiceProxy.getMultipleDropWaybillPdf(pointId).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  DownloadInvoiceReportPdf(InvoiceId: number): void {
    this._InvoiceReportServiceProxy.downloadInvoiceReportPdf(InvoiceId).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
