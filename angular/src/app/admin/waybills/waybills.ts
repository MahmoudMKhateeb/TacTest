import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { WaybillsServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  templateUrl: './waybills.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class waybillsComponent extends AppComponentBase {
  constructor(injector: Injector, private _fileDownloadService: FileDownloadService, private _waybillsServiceProxy: WaybillsServiceProxy) {
    super(injector);
  }

  platenumber = '2121';

  DownloadSingleDropWaybillPdf(platenumber: string): void {
    this._waybillsServiceProxy.getSingleDropWaybillPdf(this.platenumber).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  DownloadMultipleDropWaybillPdf(): void {
    this._waybillsServiceProxy.getMultipleDropWaybillPdf().subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  DownloadMasterWaybillPdf(): void {
    this._waybillsServiceProxy.getMasterWaybillPdf().subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}